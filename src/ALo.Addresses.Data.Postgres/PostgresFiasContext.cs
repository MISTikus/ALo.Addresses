using ALo.Addresses.Data.Models;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            if (!cancellationToken.IsCancellationRequested)
                this.BulkCopy(toInsert);
        }
    }
}
