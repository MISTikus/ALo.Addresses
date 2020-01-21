using ALo.Addresses.Data;
using ALo.Addresses.FiasUpdater.Configuration;
using ALo.Addresses.FiasUpdater.Fias.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
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
        private readonly FiasReader reader;
        private readonly Func<FiasContext> contextFactory;
        private readonly ILogger<FiasUpdater> logger;
        private readonly Stopwatch watcher;
        private readonly Func<Type, XmlSerializer> serializerFactory;
        private readonly ConcurrentQueue<AddressObject> queue;
        private Task[] tasks;

        public FiasUpdater(IOptions<Source> sourceOptions, FiasReader reader, Func<FiasContext> contextFactory, ILogger<FiasUpdater> logger)
        {
            this.sourceOptions = sourceOptions;
            this.reader = reader;
            this.contextFactory = contextFactory;
            this.logger = logger;
            this.watcher = new Stopwatch();
            this.reader.ProgressChanged += (s, a) => logger.LogInformation($"Progress: {a.ToString("F2")}%. " +
                $"Total: {TimeSpan.FromSeconds(100 * ((int)this.watcher.Elapsed.TotalSeconds) / a).ToString(@"hh\:mm\:ss")} " +
                $"Left: {(TimeSpan.FromSeconds(100 * ((int)this.watcher.Elapsed.TotalSeconds) / a) - this.watcher.Elapsed).ToString(@"hh\:mm\:ss")}");
            this.serializerFactory = t => new XmlSerializer(t);
            this.queue = new ConcurrentQueue<AddressObject>();
        }

        public async Task StopAsync(CancellationToken cancellationToken) { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.tasks = Enumerable.Range(0, 20).Select(x => Task.Run(() => Worker(cancellationToken))).ToArray();

            //Console.OutputEncoding = System.Text.Encoding.UTF8;

            //var fileName = @"D:\Downloads\Abyss\Fias\some.xml";
            var fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_ADDROBJ_20191201_203452ff-36b0-4d2f-956d-f73ec29b2440.XML";
            this.watcher.Start();
            await foreach (var data in this.reader.Read<AddressObject>(fileName, "AddressObjects", this.serializerFactory(typeof(AddressObject)), cancellationToken))
            {
                this.queue.Enqueue(data);
            }
            this.logger.LogInformation($"Elapsed: {this.watcher.Elapsed.ToString(@"hh\:mm\:ss")}");

            //fileName = @"D:\Downloads\Abyss\Fias\some_h.xml";
            fileName = @"D:\Downloads\Abyss\Fias\Xml\AS_HOUSE_20191201_f9a27344-f645-452c-8019-ae1354554774.XML";
            this.watcher.Restart();

            //await foreach (var data in this.reader.Read<Models.HouseObject>(fileName, "Houses", this.serializerFactory(typeof(Models.HouseObject))))
            //{
            //}

            this.logger.LogInformation($"Elapsed: {this.watcher.Elapsed.ToString(@"hh\:mm\:ss")}");

            Task.WaitAll(this.tasks);
        }

        private void Worker(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (this.queue.TryDequeue(out var address))
                        AddAddress(address, cancellationToken).Wait();
                }
                catch (Exception e)
                {
                    this.logger.LogWarning(e, "Failed to save address");
                }
                try { Task.Delay(10, cancellationToken).Wait(); }
                catch { }
            }
        }

        private async Task AddAddress(AddressObject data, CancellationToken cancellationToken)
        {
            using var context = this.contextFactory();
            if (await context.Addresses.AnyAsync(x => x.Id == data.Id, cancellationToken))
                return;
            context.Addresses.Add(new Data.Models.Address
            {
                Id = data.Id,
                AddressId = data.GlobalId,
                ParentAddressId = data.ParentId,
                Name = data.FormalName,
                TypeShortName = data.ShortTypeName,
                Level = (byte)data.Level,
                ActualityStatus = (byte)data.ActualityStatus,
                DivisionType = (byte)data.DivisionType,
            });
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
