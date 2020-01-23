using ALo.Addresses.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ALo.Addresses.Data
{
    public abstract class FiasContext : DbContext
    {
        public FiasContext(DbContextOptions options) : base(options) => ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<House> Houses { get; set; }

        public abstract Task InsertAll<T>(List<T> toInsert, CancellationToken cancellationToken) where T : class;
    }
}
