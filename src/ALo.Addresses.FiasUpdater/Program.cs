using ALo.Addresses.Data;
using ALo.Addresses.Data.Models;
using ALo.Addresses.Data.SqLite;
using ALo.Addresses.Data.SqlServer;
using ALo.Addresses.FiasUpdater.Configuration;
using ALo.Addresses.FiasUpdater.Fias;
using ALo.Addresses.FiasUpdater.Fias.Models;
using ALo.Addresses.FiasUpdater.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater
{
    internal static class Program
    {
        private static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configHost => configHost
                .SetBasePath(Debugger.IsAttached ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
                .AddJsonFile("appsettings.json", true, true)
                //.AddEnvironmentVariables(prefix: "PREFIX_")
                .AddCommandLine(args)
            )
            .ConfigureServices((hostContext, services) =>
            {
                services
                    .ConfigureOptions<Source>(hostContext.Configuration)
                    .AddData(hostContext.Configuration)
                    .AddSingleton<IQueueFacade, QueueFacade>()
                    .AddTransient<ISystemFacade, SystemFacade>()
                    .AddTransient<FiasReader>()
                    .AddTransient<Fias.FiasUpdater>()
                    .AddTransient<HouseHandler>()
                    .AddTransient<AddressHandler>()
                    .AddSingleton<IDictionary<Type, IHandler>>(s => new Dictionary<Type, IHandler>
                    {
                        [typeof(HouseObject)] = s.GetRequiredService<HouseHandler>(),
                        [typeof(AddressObject)] = s.GetRequiredService<AddressHandler>(),
                        [typeof(House[])] = s.GetRequiredService<HouseHandler>(),
                        [typeof(Address[])] = s.GetRequiredService<AddressHandler>(),
                    })
                    .AddLogging(c => c.AddConsole());

                if (args.Contains("--install"))
                {
                    using var provider = services.BuildServiceProvider();
                    MigrateDatabase(provider);
                    services.AddHostedService<ApplicationStopper>();
                    return;
                }

                services.AddSingleton(s => new Arguments
                {
                    Addresses = args.Contains("-a") || args.Contains("--addresses"),
                    Houses = args.Contains("-h") || args.Contains("--houses"),
                    Skip = args.Any(x => x.StartsWith("/s"))
                        ? hostContext.Configuration.GetValue<int>("s")
                        : args.Any(x => x.StartsWith("--skip"))
                            ? hostContext.Configuration.GetValue<int>("skip")
                            : 0,
                    Take = args.Any(x => x.StartsWith("/t"))
                        ? hostContext.Configuration.GetValue<int>("t")
                        : args.Any(x => x.StartsWith("--take"))
                            ? hostContext.Configuration.GetValue<int>("take")
                            : -1
                });

                services.AddHostedService<Fias.FiasUpdater>();
            });

        private static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            var dataOptions = new Configuration.Data();
            configuration.Bind(nameof(Configuration.Data), dataOptions);

            var contextRegistrationMap = new Dictionary<Provider, Action>
            {
                [Provider.SqLite] = () =>
                {
                    services.AddDbContext<SqLiteFiasContext>((s, c) => c
                        .UseSqlite(dataOptions.ConnectionString), ServiceLifetime.Transient, ServiceLifetime.Transient);
                    services.AddSingleton<Func<FiasContext>>(p => () => p.GetService<SqLiteFiasContext>());
                },
                [Provider.SqlServer] = () =>
                {
                    services.AddDbContext<SqlServerFiasContext>((s, c) => c
                        .UseSqlServer(dataOptions.ConnectionString), ServiceLifetime.Transient, ServiceLifetime.Transient);
                    services.AddSingleton<Func<FiasContext>>(p => () => p.GetService<SqlServerFiasContext>());
                },
                [Provider.Postgres] = () =>
                {
                    services.AddDbContext<PostgresFiasContext>((s, c) => c
                        .UseNpgsql(dataOptions.ConnectionString), ServiceLifetime.Transient, ServiceLifetime.Transient);
                    services.AddSingleton<Func<FiasContext>>(p => () => p.GetService<PostgresFiasContext>());
                },
                // ToDo: add more
            };

            contextRegistrationMap[dataOptions.Provider]();
            return services;
        }

        private static void MigrateDatabase(IServiceProvider provider)
        {
            var logger = provider.GetService<ILogger<FiasContext>>();
            using var context = provider.GetRequiredService<Func<FiasContext>>()();
            logger.LogInformation("Migrating database...");
            try
            {
                context.Database.Migrate();
                logger.LogInformation("Migration completed...");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Migration failed...");
            }
        }

        private static IServiceCollection ConfigureOptions<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class => services
            .Configure<T>(configuration.GetSection(typeof(T).Name));

        private static IServiceCollection AddFactory<T>(this IServiceCollection services) where T : class => services
            .AddTransient<T>()
            .AddTransient<Func<T>>(s => () => s.GetService<T>());
    }
}
