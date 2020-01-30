using ALo.Addresses.Data.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.Data.SqLite
{
    public class SqLiteFiasContext : FiasContext
    {
        public SqLiteFiasContext(DbContextOptions<SqLiteFiasContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Address>()
                .HasIndex(a => new { a.AddressId, a.ActualityStatus });
            builder.Entity<Address>()
                .HasIndex(a => new { a.ParentAddressId, a.ActualityStatus });

            builder.Entity<House>()
                .HasIndex(h => new { h.HouseId, h.EndDate });
            builder.Entity<House>()
                .HasIndex(h => new { h.AddressId, h.EndDate });
        }

        public override async Task InsertAll<T>(List<T> toInsert, CancellationToken cancellationToken) where T : class => await this
            .BulkInsertAsync(toInsert, cancellationToken: cancellationToken);
        public override async Task<IEnumerable<TKey>> CheckExistence<TModel, TKey>(IEnumerable<TKey> keys, bool isExists, CancellationToken cancellationToken) => await
            Set<TModel>()
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}
