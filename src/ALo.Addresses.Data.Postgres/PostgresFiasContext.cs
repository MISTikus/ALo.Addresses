using ALo.Addresses.Data.Models;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.Data.SqlServer
{
    public class PostgresFiasContext : FiasContext
    {
        public PostgresFiasContext(DbContextOptions<PostgresFiasContext> options) : base(options) => LinqToDBForEFTools.Initialize();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Address>()
                .HasIndex(a => a.AddressId)
                .IncludeProperties(a => a.ActualityStatus);
            builder.Entity<Address>()
                .HasIndex(a => a.ParentAddressId)
                .IncludeProperties(a => a.ActualityStatus);

            builder.Entity<House>()
                .HasIndex(h => h.HouseId)
                .IncludeProperties(h => h.EndDate);
            builder.Entity<House>()
                .HasIndex(h => h.AddressId)
                .IncludeProperties(h => h.EndDate);
        }

        public override async Task InsertAll<T>(List<T> toInsert, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!cancellationToken.IsCancellationRequested)
                this.BulkCopy(toInsert);
        }

        public override async Task<IEnumerable<TKey>> CheckExistence<TModel, TKey>(IEnumerable<TKey> keys, bool isExists, CancellationToken cancellationToken)
        {
            IEnumerable<TKey> exists(IGrouping<int, TKey> group) => Set<TModel>().Where(x => group.Contains(x.Id)).Select(x => x.Id).ToList();
            IEnumerable<TKey> notExists(IGrouping<int, TKey> group) => group.Where(x => !Set<TModel>().Any(s => s.Id.Equals(x))).ToList();

            var i = 0;
            return keys
                .GroupBy(x => i++ % 10000)
                .SelectMany(isExists ? exists : (Func<IGrouping<int, TKey>, IEnumerable<TKey>>)notExists)
                .ToList();
        }
    }
}
