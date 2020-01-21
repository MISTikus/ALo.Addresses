using ALo.Addresses.Data.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
