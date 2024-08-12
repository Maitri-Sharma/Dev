using Microsoft.EntityFrameworkCore;
using Puma.DataLayer.DatabaseModel;
using Puma.DataLayer.DatabaseModel.KspuDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure
{
    public class KspuDBContext : DbContext
    {
        public KspuDBContext(DbContextOptions<KspuDBContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql("Server=puma-dev-norwayeast.postgres.database.azure.com;Username=puma_dev_admin@puma-dev-norwayeast;Password=z$mCX#IMsJ8@ykS2t8Ukz)xNAU!_h[x[;Integrated Security=true;SSLmode=Require;Port=5432;Database=puma;pooling=true;", builder =>
        //    {
        //        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        //    });

        //}

        public DbSet<AddressPointsState> addresspointsstates { get; set; }

        public DbSet<utvalg> utvalg { get; set; }

        public DbSet<utvalglist> utvalgList { get; set; }

        public DbSet<UtvalgListModification> UtvalgListModifications { get; set; }

        public DbSet<UtvalgModifications> UtvalgModifications { get; set; }

        //public DbSet<selectionDistribution> selectionDistributions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("kspu_db");
            base.OnModelCreating(builder);
        }


    }
}
