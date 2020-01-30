using ALo.Addresses.Data;
using ALo.Addresses.Data.Models;
using ALo.Addresses.FiasUpdater.Fias.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.FiasUpdater.Fias
{
    internal class AddressHandler : IHandler<AddressObject>, IHandler<Address[]>
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

        public async Task HandleAsync(Address[] items, FiasContext context, CancellationToken cancellationToken)
        {
            var ids = items.Select(x => x.Id).ToList();
            var existingIds = await context.CheckExistence<Address, Guid>(ids, cancellationToken: cancellationToken);
            //.Addresses.AsNoTracking().Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync(cancellationToken);
            var toInsert = items.Where(x => !existingIds.Contains(x.Id)).OrderBy(x => x.Id).ToList();

            await context.InsertAll(toInsert, cancellationToken: cancellationToken);
        }
    }
}
