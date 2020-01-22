using ALo.Addresses.Data;
using ALo.Addresses.FiasUpdater.Fias;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater.Infrastructure
{
    internal class QueueFacade : IQueueFacade
    {
        private readonly ConcurrentQueue<Func<FiasContext, Task>> queue;
        private readonly ConcurrentDictionary<Type, Func<FiasContext, object, Task>> workers;
        private readonly IDictionary<Type, IHandler> handlers;
        private readonly Func<FiasContext> contextFactory;
        private readonly ILogger<QueueFacade> logger;
        private readonly CancellationTokenSource cancellation;
        private readonly Task[] tasks;
        private int lastQueueLength;
        public int QueueLength { get; set; }

        public QueueFacade(IDictionary<Type, IHandler> handlers, Func<FiasContext> contextFactory, ILogger<QueueFacade> logger)
        {
            this.queue = new ConcurrentQueue<Func<FiasContext, Task>>();
            this.workers = new ConcurrentDictionary<Type, Func<FiasContext, object, Task>>();
            this.handlers = handlers;
            this.contextFactory = contextFactory;
            this.logger = logger;
            this.cancellation = new CancellationTokenSource();
            this.tasks = Enumerable.Range(0, 10).Select(x => Task.Run(() => DequeueAsync().Wait())).ToArray();
        }


        public void Enqueue<T>(T item, CancellationToken cancellationToken)
        {
            var type = typeof(T);
            if (!this.workers.ContainsKey(type))
                this.workers.AddOrUpdate(type, t => CreateWorker<T>(cancellationToken), (t, a) => a);

            this.queue.Enqueue(c =>
            {
                if (this.workers.TryGetValue(type, out var func))
                    return func(c, item);
                else
                    this.logger.LogWarning($"Can`t add worker for type '{type.Name}'");
                return Task.CompletedTask;
            });

            QueueLength++;
        }

        private async Task DequeueAsync()
        {
            using var context = this.contextFactory();
            while (!this.cancellation.IsCancellationRequested)
            {
                if (this.queue.TryDequeue(out var func))
                    try
                    {
                        QueueLength--;
                        await func(context);
                        if (QueueLength % 1000 == 0 && QueueLength != this.lastQueueLength)
                        {
                            this.logger.LogInformation($"Item saved. Queue length: '{QueueLength}'");
                            this.lastQueueLength = QueueLength;
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.LogError(e, "Error...");
                    }
                else
                    await Task.Delay(10);
            }
        }

        private Func<FiasContext, object, Task> CreateWorker<T>(CancellationToken cancellationToken)
        {
            var type = typeof(T);
            var handler = this.handlers[type];
            var handle = handler.GetType().GetMethod("HandleAsync");
            return (c, o) => !cancellationToken.IsCancellationRequested
                    ? (Task)handle.Invoke(handler, new[] { o, c, cancellationToken })
                    : Task.FromCanceled(cancellationToken);
        }

        public Task Awaiter() => Task.WhenAll(this.tasks);
        public void Cancel() => this.cancellation.Cancel();
    }

    internal interface IQueueFacade
    {
        int QueueLength { get; set; }
        void Enqueue<T>(T item, CancellationToken cancellationToken);
        Task Awaiter();
        void Cancel();
    }
}
