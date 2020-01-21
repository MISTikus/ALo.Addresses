using ALo.Addresses.FiasUpdater.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias
{
    internal class FiasUpdater : IHostedService
    {
        private readonly IOptions<Source> sourceOptions;
        private readonly FiasReader reader;
        private readonly Stopwatch watcher;
        private readonly Func<Type, XmlSerializer> serializerFactory;

        public FiasUpdater(IOptions<Source> sourceOptions, FiasReader reader)
        {
            this.sourceOptions = sourceOptions;
            this.reader = reader;
            this.watcher = new Stopwatch();
            this.reader.ProgressChanged += (s, a) => Console.WriteLine($"Progress: {a.ToString("F2")}%. " +
                $"Total: {TimeSpan.FromSeconds(100 * ((int)this.watcher.Elapsed.TotalSeconds) / a).ToString(@"hh\:mm\:ss")} " +
                $"Left: {(TimeSpan.FromSeconds(100 * ((int)this.watcher.Elapsed.TotalSeconds) / a) - this.watcher.Elapsed).ToString(@"hh\:mm\:ss")}");
            this.serializerFactory = t => new XmlSerializer(t);
        }

        public async Task StopAsync(CancellationToken cancellationToken) { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileName = @"D:\Downloads\Abyss\Fias\some.xml";
            this.watcher.Start();
            //await foreach (var data in this.reader.Read<Models.AddressObject>(fileName, "AddressObjects", this.serializerFactory(typeof(Models.AddressObject))))
            //{
            //    Console.WriteLine($"{i} : {data.Level}: {data.ShortTypeName} {data.OfficialName}");
            //    i++;
            //}
            Console.WriteLine($"Elapsed: {this.watcher.Elapsed.ToString(@"hh\:mm\:ss")}");

            //fileName = @"D:\Downloads\Abyss\Fias\some_h.xml";
            fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_HOUSE_20191201_f9a27344-f645-452c-8019-ae1354554774.XML";
            this.watcher.Restart();

            await foreach (var data in this.reader.Read<Models.HouseObject>(fileName, "Houses", this.serializerFactory(typeof(Models.HouseObject))))
            {
                //i++;
            }

            Console.WriteLine($"Elapsed: {this.watcher.Elapsed.ToString(@"hh\:mm\:ss")}");
        }
    }
}
