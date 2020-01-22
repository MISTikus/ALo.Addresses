using ALo.Addresses.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater.Fias
{
    internal interface IHandler
    {
    }

    internal interface IHandler<T> : IHandler
    {
        Task HandleAsync(T item, FiasContext context, CancellationToken cancellationToken);
    }
}
