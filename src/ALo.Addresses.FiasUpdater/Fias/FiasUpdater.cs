using ALo.Addresses.FiasUpdater.Configuration;
using ALo.Addresses.FiasUpdater.Fias.Models;
using ALo.Addresses.FiasUpdater.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly FiasReader addressReader;
        private readonly FiasReader houseReader;
        private readonly ILogger<FiasUpdater> logger;
        private readonly IQueueFacade queue;
        private readonly Func<Type, XmlSerializer> serializerFactory;

        public FiasUpdater(IOptions<Source> sourceOptions, FiasReader addressReader, FiasReader houseReader,
            ILogger<FiasUpdater> logger, IQueueFacade queueFacade)
        {
            this.sourceOptions = sourceOptions;
            this.addressReader = addressReader;
            this.houseReader = houseReader;
            this.logger = logger;
            this.queue = queueFacade;
            this.serializerFactory = t => new XmlSerializer(t);
        }

        public async Task StopAsync(CancellationToken cancellationToken) { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => this.queue.Cancel());

            var addressTask = ReadAdresses(cancellationToken);
            var houseTask = ReadHouses(cancellationToken);

            await Task.WhenAll(addressTask, houseTask, this.queue.Awaiter());
        }

        private async Task ReadAdresses(CancellationToken cancellationToken)
        {
            var addressWatcher = new Stopwatch();
            this.addressReader.ProgressChanged += (s, a) => this.logger.LogInformation($"Reading addresses progress: {a.ToString("F0")}%. " +
                $"Total: {TimeSpan.FromSeconds(100 * ((int)addressWatcher.Elapsed.TotalSeconds) / a).ToString(@"hh\:mm\:ss")} " +
                $"Left: {(TimeSpan.FromSeconds(100 * ((int)addressWatcher.Elapsed.TotalSeconds) / a) - addressWatcher.Elapsed).ToString(@"hh\:mm\:ss")}");
            await Task.Run(async () =>
            {
                addressWatcher.Start();
                //var fileName = @"D:\Downloads\Abyss\Fias\some.xml";
                var fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_ADDROBJ_20191201_203452ff-36b0-4d2f-956d-f73ec29b2440.XML";
                await foreach (var data in this.addressReader.Read<AddressObject>(fileName, "AddressObjects", this.serializerFactory(typeof(AddressObject)), cancellationToken))
                {
                    if (this.queue.QueueLength >= 1 * 1000 * 1000)
                        await Task.Delay(100);
                    this.queue.Enqueue(data, cancellationToken);
                }
                this.logger.LogInformation($"Elapsed: {addressWatcher.Elapsed.ToString(@"hh\:mm\:ss")}");
            });
        }

        private async Task ReadHouses(CancellationToken cancellationToken)
        {
            var houseWatcher = new Stopwatch();
            this.houseReader.ProgressChanged += (s, a) => this.logger.LogInformation($"Reading houses progress: {a.ToString("F2")}%. " +
                $"Total: {TimeSpan.FromSeconds(100 * ((int)houseWatcher.Elapsed.TotalSeconds) / a).ToString(@"hh\:mm\:ss")} " +
                $"Left: {(TimeSpan.FromSeconds(100 * ((int)houseWatcher.Elapsed.TotalSeconds) / a) - houseWatcher.Elapsed).ToString(@"hh\:mm\:ss")}");
            await Task.Run(async () =>
            {
                houseWatcher.Start();
                //var fileName = @"D:\Downloads\Abyss\Fias\some_h.xml";
                var fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_HOUSE_20191201_f9a27344-f645-452c-8019-ae1354554774.XML";
                await foreach (var data in this.houseReader.Read<HouseObject>(fileName, "Houses", this.serializerFactory(typeof(HouseObject)), cancellationToken))
                {
                    if (this.queue.QueueLength >= 2 * 1000 * 1000)
                        await Task.Delay(100);
                    this.queue.Enqueue(data, cancellationToken);
                }
                this.logger.LogInformation($"Elapsed: {houseWatcher.Elapsed.ToString(@"hh\:mm\:ss")}");
            });
        }
    }
}
