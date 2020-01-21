using ALo.Addresses.Data;
using ALo.Addresses.Data.SqLite;
using ALo.Addresses.Data.SqlServer;
using ALo.Addresses.FiasUpdater.Configuration;
using ALo.Addresses.FiasUpdater.Fias;
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
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater
{
    internal static class Program
    {
        private static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

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
                    .AddTransient<ISystemFacade, SystemFacade>()
                    .AddTransient<FiasReader>()
                    .AddTransient<Fias.FiasUpdater>()
                    .AddLogging(c => c.AddConsole());

                if (args.Contains("--install"))
                {
                    using var provider = services.BuildServiceProvider();
                    MigrateDatabase(provider);
                    services.AddHostedService<ApplicationStopper>();
                    return;
                }

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
                    services.AddTransient<Func<FiasContext>>(p => () => p.GetService<SqLiteFiasContext>());
                },
                [Provider.SqlServer] = () =>
                {
                    services.AddDbContext<SqlServerFiasContext>((s, c) => c
                        .UseSqlServer(dataOptions.ConnectionString), ServiceLifetime.Transient, ServiceLifetime.Transient);
                    services.AddTransient<Func<FiasContext>>(p => () => p.GetService<SqlServerFiasContext>());
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

        private class ApplicationStopper : IHostedService
        {
            private readonly IHostApplicationLifetime applicationLifetime;

            public ApplicationStopper(IHostApplicationLifetime applicationLifetime) => this.applicationLifetime = applicationLifetime;

            public async Task StartAsync(CancellationToken cancellationToken) => this.applicationLifetime.StopApplication();
            public async Task StopAsync(CancellationToken cancellationToken) { }
        }
    }
}
