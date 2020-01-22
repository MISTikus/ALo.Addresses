using ALo.Addresses.Data;
using ALo.Addresses.FiasUpdater.Fias.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater.Fias
{
    internal class AddressHandler : IHandler<AddressObject>
    {
        private readonly Func<FiasContext> contextFactory;

        public AddressHandler(Func<FiasContext> contextFactory) => this.contextFactory = contextFactory;

        public async Task HandleAsync(AddressObject item, FiasContext context, CancellationToken cancellationToken)
        {
            //using var context = this.contextFactory();
            if (await context.Addresses.AnyAsync(x => x.Id == item.Id, cancellationToken))
                return;
            await context.Addresses.AddAsync(new Data.Models.Address
            {
                Id = item.Id,
                AddressId = item.GlobalId,
                ParentAddressId = item.ParentId,
                Name = item.FormalName,
                TypeShortName = item.ShortTypeName,
                Level = (byte)item.Level,
                ActualityStatus = (byte)item.ActualityStatus,
                DivisionType = (byte)item.DivisionType,
            });
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
