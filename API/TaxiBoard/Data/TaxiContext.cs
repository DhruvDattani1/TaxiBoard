using Microsoft.EntityFrameworkCore;
using TaxiBoard.Models;

namespace TaxiBoard.Data
{
    public class TaxiBoardContext : DbContext
    {
        public TaxiBoardContext(DbContextOptions<TaxiBoardContext> options)
            : base(options)
        {
        }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<RateCode> RateCodes { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<TaxiZone> TaxiZones { get; set; }
        public DbSet<YellowTripData> YellowTripData { get; set; }


    }
    using Microsoft.EntityFrameworkCore;
using TaxiBoard.Models;

namespace TaxiBoard.Data
{
    public class TaxiBoardContext : DbContext
    {
        public TaxiBoardContext(DbContextOptions<TaxiBoardContext> options)
            : base(options)
        {
        }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<RateCode> RateCodes { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<TaxiZone> TaxiZones { get; set; }
        public DbSet<YellowTripData> YellowTripData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<YellowTripData>()
                .HasOne(t => t.Vendor)
                .WithMany(v => v.Trips)
                .HasForeignKey(t => t.VendorId);

            modelBuilder.Entity<YellowTripData>()
                .HasOne(t => t.RateCode)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RateCodeId);

            modelBuilder.Entity<YellowTripData>()
                .HasOne(t => t.PaymentType)
                .WithMany(p => p.Trips)
                .HasForeignKey(t => t.PaymentTypeId);

            modelBuilder.Entity<YellowTripData>()
                .HasOne(t => t.PickupZone)
                .WithMany(z => z.PickupTrips)
                .HasForeignKey(t => t.PULocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<YellowTripData>()
                .HasOne(t => t.DropoffZone)
                .WithMany(z => z.DropoffTrips)
                .HasForeignKey(t => t.DOLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

}
