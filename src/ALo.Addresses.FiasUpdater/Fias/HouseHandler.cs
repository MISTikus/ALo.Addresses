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
    internal class HouseHandler : IHandler<HouseObject>, IHandler<House[]>
    {
        private readonly Func<FiasContext> contextFactory;

        public HouseHandler(Func<FiasContext> contextFactory) => this.contextFactory = contextFactory;

        public async Task HandleAsync(HouseObject item, FiasContext context, CancellationToken cancellationToken)
        {
            //using var context = this.contextFactory();
            if (await context.Houses.AnyAsync(x => x.Id == item.Id, cancellationToken))
                return;
            await context.Houses.AddAsync(new Data.Models.House
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
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task HandleAsync(House[] items, FiasContext context, CancellationToken cancellationToken)
        {
            var ids = items.Select(x => x.Id).ToList();
            var existingIds = await context.Houses.AsNoTracking().Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync();
            var toInsert = items.Where(x => !existingIds.Contains(x.Id)).ToList();

            await context.InsertAll(toInsert, cancellationToken: cancellationToken);
        }
    }
}
