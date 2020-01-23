using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater
{
    internal class ApplicationStopper : IHostedService
        {
            private readonly IHostApplicationLifetime applicationLifetime;

            public ApplicationStopper(IHostApplicationLifetime applicationLifetime) => this.applicationLifetime = applicationLifetime;

            public async Task StartAsync(CancellationToken cancellationToken) => this.applicationLifetime.StopApplication();
            public async Task StopAsync(CancellationToken cancellationToken) { }
        }
}
