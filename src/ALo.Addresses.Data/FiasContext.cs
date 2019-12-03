using ALo.Addresses.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ALo.Addresses.Data
{
    public class FiasContext : DbContext
    {
        public FiasContext(DbContextOptions<FiasContext> options) : base(options) { }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Shorting> Shortings { get; set; }
    }
}
