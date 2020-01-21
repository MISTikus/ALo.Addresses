using ALo.Addresses.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ALo.Addresses.Data
{
    public abstract class FiasContext : DbContext
    {
        public FiasContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<House> Houses { get; set; }
    }
}
