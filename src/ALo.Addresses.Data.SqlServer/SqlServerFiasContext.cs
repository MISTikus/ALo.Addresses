using ALo.Addresses.Data.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.Data.SqlServer
{
    public class SqlServerFiasContext : FiasContext
    {
        public SqlServerFiasContext(DbContextOptions<SqlServerFiasContext> options) : base(options)
        {
        }

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

        public override async Task InsertAll<T>(List<T> toInsert, CancellationToken cancellationToken) where T : class => await this
            .BulkInsertAsync(toInsert, cancellationToken: cancellationToken);

        public override async Task<IEnumerable<TKey>> CheckExistence<TModel, TKey>(IEnumerable<TKey> keys, bool isExists, CancellationToken cancellationToken)
        {
            IEnumerable<TKey> exists(IEnumerable<TKey> group) => Set<TModel>().Where(x => group.Contains(x.Id)).Select(x => x.Id).ToList();
            IEnumerable<TKey> notExists(IEnumerable<TKey> group) => group.Where(x => !Set<TModel>().Any(s => s.Id.Equals(x))).ToList();
            var check = isExists ? exists : (Func<IEnumerable<TKey>, IEnumerable<TKey>>)notExists;

            var i = 0;
            var count = keys.Count();
            return keys
                .Select(k => new { i = i++, v = k })
                .ToList()
                .GroupBy(x => new { i = x.i / 10000 })
                .SelectMany(g => check(g.Select(x => x.v)))
                .ToList();
        }
    }
}
