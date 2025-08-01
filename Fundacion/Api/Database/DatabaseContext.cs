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

            modelBuilder.Entity<VolunteerRequest>()
                .HasOne(r => r.Volunteer)
                .WithMany()
                .HasForeignKey(r => r.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VolunteerRequest>()
                .HasOne(r => r.Approver)
                .WithMany()
                .HasForeignKey(r => r.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== NUEVAS CONFIGURACIONES PARA VOLUNTEER HOURS =====

            // VolunteerHours configuraciones mejoradas
            modelBuilder.Entity<VolunteerHours>()
                .Property(vh => vh.State)
                .HasConversion<string>();

            modelBuilder.Entity<VolunteerHours>()
                .Property(vh => vh.TotalHours)
                .HasColumnType("decimal(5,2)"); // Máximo 999.99 horas

            modelBuilder.Entity<VolunteerHours>()
                .HasOne(vh => vh.VolunteerRequest)
                .WithMany()
                .HasForeignKey(vh => vh.VolunteerRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VolunteerHours>()
                .HasOne(vh => vh.Approver)
                .WithMany()
                .HasForeignKey(vh => vh.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para optimizar consultas
            modelBuilder.Entity<VolunteerHours>()
                .HasIndex(vh => new { vh.VolunteerRequestId, vh.Date })
                .HasDatabaseName("IX_VolunteerHours_Request_Date");

            modelBuilder.Entity<VolunteerHours>()
                .HasIndex(vh => vh.State)
                .HasDatabaseName("IX_VolunteerHours_State");

            // Restricción: No más de un registro por voluntario por día
            modelBuilder.Entity<VolunteerHours>()
                .HasIndex(vh => new { vh.VolunteerRequestId, vh.Date })
                .IsUnique()
                .HasDatabaseName("UX_VolunteerHours_OnePerDay");

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
        public DbSet<VolunteerRequest> VolunteerRequests { get; set; }
        public DbSet<VolunteerHours> VolunteerHours { get; set; }
    }
}
