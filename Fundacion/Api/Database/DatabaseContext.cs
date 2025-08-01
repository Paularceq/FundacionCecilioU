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

            // Enum conversion
            modelBuilder.Entity<Product>()
                .Property(p => p.UnitOfMeasure)
                .HasConversion<string>();

            modelBuilder.Entity<OutgoingDonation>()
                .HasOne(d => d.Requester)
                .WithMany()
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OutgoingDonation>()
                .HasOne(d => d.Recipient)
                .WithMany()
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OutgoingDonation>()
                .HasOne(d => d.Approver)
                .WithMany()
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);
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
        public DbSet<SolicitudBeca> SolicitudesBeca { get; set; }
        public DbSet<OutgoingDonation> OutgoingDonations { get; set; }
    }
}
