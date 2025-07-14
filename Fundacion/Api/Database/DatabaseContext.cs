using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ACTIVITY DONATION
            modelBuilder.Entity<ActivityDonation>()
                .HasOne(ad => ad.Donation)
                .WithMany() // o WithOne() si es uno a uno
                .HasForeignKey(ad => ad.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            // MONETARY DONATION
            modelBuilder.Entity<MonetaryDonation>()
                .HasOne(md => md.Donation)
                .WithMany() // si es uno a uno cambia a WithOne()
                .HasForeignKey(md => md.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            // PRODUCTS DONATION
            modelBuilder.Entity<ProductsDonation>()
                .HasOne(pd => pd.Donation)
                .WithMany() // o WithOne()
                .HasForeignKey(pd => pd.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enum conversion
            modelBuilder.Entity<Product>()
                .Property(p => p.UnitOfMeasure)
                .HasConversion<string>();
        }


        // Define DbSet properties for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<ActivityDonation> ActivityDonations { get; set; }
        public DbSet<Donation> Donations { get; set; }

        

        public DbSet<MonetaryDonation> MonetaryDonations { get; set; }

        public DbSet<ProductsDonation> ProductsDonations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
    }
}
