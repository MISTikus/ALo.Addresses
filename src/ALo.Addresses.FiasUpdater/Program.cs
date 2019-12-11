using ALo.Addresses.Data;
using ALo.Addresses.FiasUpdater.Configuration;
using ALo.Addresses.FiasUpdater.Fias;
using ALo.Addresses.FiasUpdater.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Debugger.IsAttached ? Directory.GetCurrentDirectory() : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var services = CompileServices(config);
            var provider = services.BuildServiceProvider();

            //using (var context = provider.GetRequiredService<Func<FiasContext>>()())
            //{
            //    await context.Database.MigrateAsync();
            //}

            var updater = provider.GetRequiredService<Fias.FiasUpdater>();
            await updater.Update();
        }

        private static IServiceCollection CompileServices(IConfiguration configuration) => new ServiceCollection()
            .ConfigureOptions<Source>(configuration)
            .AddData(configuration)
            .AddTransient<ISystemFacade, SystemFacade>()
            .AddTransient<FiasReader>()
            .AddTransient<Fias.FiasUpdater>();

        private static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            var dataOptions = new Configuration.Data();
            configuration.Bind(nameof(Configuration.Data), dataOptions);

            var contextSwitch = new Dictionary<Provider, Action<DbContextOptionsBuilder>>
            {
                [Provider.SqLite] = b => b.UseSqlite(dataOptions.ConnectionString),
                [Provider.SqlServer] = b => b.UseSqlServer(dataOptions.ConnectionString),
                // ToDo: add more
            };
            services
                .AddDbContext<FiasContext>(c => contextSwitch[dataOptions.Provider](c))
                .AddTransient<Func<FiasContext>>(p => () => p.GetService<FiasContext>());

            return services;
        }

        private static IServiceCollection ConfigureOptions<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class => services
            .Configure<T>(configuration.GetSection(typeof(T).Name));

        private static IServiceCollection AddFactory<T>(this IServiceCollection services) where T : class => services
            .AddTransient<T>()
            .AddTransient<Func<T>>(s => () => s.GetService<T>());
    }
}
