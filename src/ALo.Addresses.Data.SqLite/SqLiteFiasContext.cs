using ALo.Addresses.Data.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
