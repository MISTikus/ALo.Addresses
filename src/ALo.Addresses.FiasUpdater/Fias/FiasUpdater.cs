using ALo.Addresses.Data.Models;
using ALo.Addresses.FiasUpdater.Configuration;
using ALo.Addresses.FiasUpdater.Fias.Models;
using ALo.Addresses.FiasUpdater.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias
{
    internal class FiasUpdater : IHostedService
    {
        private readonly IOptions<Source> sourceOptions;
        private readonly ISystemFacade systemFacade;
        private readonly FiasReader addressReader;
        private readonly FiasReader houseReader;
        private readonly ILogger<FiasUpdater> logger;
        private readonly IQueueFacade queue;
        private readonly Arguments arguments;
        private readonly IHostApplicationLifetime stopper;
        private readonly Func<Type, XmlSerializer> serializerFactory;

        public FiasUpdater(IOptions<Source> sourceOptions, ISystemFacade systemFacade, FiasReader addressReader, FiasReader houseReader,
            ILogger<FiasUpdater> logger, IQueueFacade queueFacade, Arguments arguments, IHostApplicationLifetime stopper)
        {
            this.sourceOptions = sourceOptions;
            this.systemFacade = systemFacade;
            this.addressReader = addressReader;
            this.houseReader = houseReader;
            this.logger = logger;
            this.queue = queueFacade;
            this.arguments = arguments;
            this.stopper = stopper;
            this.serializerFactory = t => new XmlSerializer(t);
        }

        public async Task StopAsync(CancellationToken cancellationToken) { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => this.queue.Cancel());

            if (string.IsNullOrWhiteSpace(this.sourceOptions.Value?.Folder))
                throw new ArgumentNullException(nameof(Source));

            // ToDo: unpack rar archive
            var files = this.systemFacade.GetFilesAsync(this.sourceOptions.Value.Folder);

            Task addressTask = Task.CompletedTask, houseTask = Task.CompletedTask;
            if (this.arguments.Addresses)
                addressTask = ReadAdresses(files.First(x => x.Contains("AS_ADDROBJ")), cancellationToken);
            if (this.arguments.Houses)
                houseTask = ReadHouses(files.First(x => x.Contains("AS_HOUSE")), cancellationToken);

            await Task.WhenAll(addressTask, houseTask, this.queue.Awaiter());
            this.stopper.StopApplication();
        }

        private async Task ReadAdresses(string fileName, CancellationToken cancellationToken)
        {
            var addressWatcher = new Stopwatch();
            this.addressReader.ProgressChanged += (s, a) => this.logger.LogInformation($"Reading addresses progress: {a.ToString("F0")}%. " +
                $"Total: {TimeSpan.FromSeconds(100 * ((int)addressWatcher.Elapsed.TotalSeconds) / a).ToString(@"hh\:mm\:ss")} " +
                $"Left: {(TimeSpan.FromSeconds(100 * ((int)addressWatcher.Elapsed.TotalSeconds) / a) - addressWatcher.Elapsed).ToString(@"hh\:mm\:ss")}");
            await Task.Run(async () =>
            {
                addressWatcher.Start();
                var chunk = new List<AddressObject>();

                await foreach (var data in this.addressReader.Read<AddressObject>(fileName, "AddressObjects", this.serializerFactory(typeof(AddressObject)),
                    cancellationToken, skip: this.arguments.Skip, take: this.arguments.Take))
                {
                    if (this.queue.QueueLength >= 5 * 1000)
                        await Task.Delay(100);
                    chunk.Add(data);
                    if (chunk.Count >= 10000)
                    {
                        this.queue.Enqueue(chunk.Select(x => ToDto(x)).ToArray(), cancellationToken);
                        chunk.Clear();
                    }
                }
                if (chunk.Count > 0)
                    this.queue.Enqueue(chunk.Select(x => ToDto(x)).ToArray(), cancellationToken);

                this.queue.Cancel(true);
                this.logger.LogInformation($"Elapsed: {addressWatcher.Elapsed.ToString(@"hh\:mm\:ss")}");
            });
        }

        private async Task ReadHouses(string fileName, CancellationToken cancellationToken)
        {
            var houseWatcher = new Stopwatch();
            this.houseReader.ProgressChanged += (s, a) => this.logger.LogInformation($"Reading houses progress: {a.ToString("F0")}%. " +
                $"Total: {TimeSpan.FromSeconds(100 * ((int)houseWatcher.Elapsed.TotalSeconds) / a).ToString(@"hh\:mm\:ss")} " +
                $"Left: {(TimeSpan.FromSeconds(100 * ((int)houseWatcher.Elapsed.TotalSeconds) / a) - houseWatcher.Elapsed).ToString(@"hh\:mm\:ss")}");
            await Task.Run(async () =>
            {
                houseWatcher.Start();
                var chunk = new List<HouseObject>();
                await foreach (var data in this.houseReader.Read<HouseObject>(fileName, "Houses", this.serializerFactory(typeof(HouseObject)),
                    cancellationToken, skip: this.arguments.Skip, take: this.arguments.Take))
                {
                    if (this.queue.QueueLength >= 5 * 1000)
                        await Task.Delay(100);
                    chunk.Add(data);
                    if (chunk.Count >= 10000)
                    {
                        this.queue.Enqueue(chunk.Select(x => ToDto(x)).ToArray(), cancellationToken);
                        chunk.Clear();
                    }
                }
                if (chunk.Count > 0)
                    this.queue.Enqueue(chunk.Select(x => ToDto(x)).ToArray(), cancellationToken);

                this.queue.Cancel(true);
                this.logger.LogInformation($"Elapsed: {houseWatcher.Elapsed.ToString(@"hh\:mm\:ss")}");
            });
        }

        private Address ToDto(AddressObject item) => new Address
        {
            Id = item.Id,
            AddressId = item.GlobalId,
            ParentAddressId = item.ParentId,
            Name = item.FormalName,
            TypeShortName = item.ShortTypeName,
            Level = (byte)item.Level,
            ActualityStatus = (byte)item.ActualityStatus,
            DivisionType = (byte)item.DivisionType,
        };
        private House ToDto(HouseObject item) => new House
        {
            Id = item.Id,
            HouseId = item.GlobalId,
            AddressId = item.AddressId,
            BuildNumber = item.BuildingNumber,
            EndDate = item.EndDate,
            HouseNumber = item.HouseNumber,
            HouseState = (byte)item.BuildingState,
            HouseType = (byte)item.OwnershipStatus,
            StructureNumber = item.StructureNumber,
        };
    }
}
