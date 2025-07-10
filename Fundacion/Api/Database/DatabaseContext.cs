using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entities here
            // Example: modelBuilder.Entity<YourEntity>().ToTable("YourTableName");
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ActivityDonation>()
                .HasOne<Donation>().WithOne()
                .HasForeignKey<ActivityDonation>(e => e.Id);

            
            modelBuilder.Entity<MonetaryDonation>()
                .HasOne<Donation>().WithOne()
                .HasForeignKey<MonetaryDonation>(e => e.Id);

            modelBuilder.Entity<ProductsDonation>()
               .HasOne<Donation>().WithOne()
               .HasForeignKey<ProductsDonation>(e => e.Id);

            modelBuilder.Entity<DonationProduct>()
               .HasOne<ProductsDonation>().WithOne()
               .HasForeignKey<DonationProduct>(e => e.Id);


            modelBuilder.Entity<Product>()
                .Property(p => p.UnitOfMeasure)
                .HasConversion<string>();
        }

        // Define DbSet properties for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<ActivityDonation> ActivityDonations { get; set; }
        public DbSet<Donation> Donations { get; set; }

        public DbSet<DonationProduct> DonationProducts { get; set; }

        public DbSet<MonetaryDonation> MonetaryDonations { get; set; }

        public DbSet<ProductsDonation> ProductsDonations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
    }
}
