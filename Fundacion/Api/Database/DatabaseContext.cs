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

            // HomeContent configuraciones
            modelBuilder.Entity<HomeContent>()
                .HasOne(hc => hc.Creator)
                .WithMany()
                .HasForeignKey(hc => hc.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HomeContent>()
                .HasIndex(hc => hc.IsActive)
                .HasDatabaseName("IX_HomeContent_IsActive");

            modelBuilder.Entity<HomeContent>()
                .HasIndex(hc => new { hc.IsActive, hc.StartDate, hc.EndDate })
                .HasDatabaseName("IX_HomeContent_Active_Dates");

            // Newsletter configuraciones
            modelBuilder.Entity<Newsletter>()
                .Property(n => n.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Newsletter>()
                .HasOne(n => n.Creator)
                .WithMany()
                .HasForeignKey(n => n.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Newsletter>()
                .HasIndex(n => n.Status)
                .HasDatabaseName("IX_Newsletter_Status");

            modelBuilder.Entity<Newsletter>()
                .HasIndex(n => n.SendDate)
                .HasDatabaseName("IX_Newsletter_SendDate");

            // NewsletterSubscription configuraciones
            modelBuilder.Entity<NewsletterSubscription>()
                .Property(ns => ns.Frequency)
                .HasConversion<string>();

            modelBuilder.Entity<NewsletterSubscription>()
                .HasIndex(ns => ns.Email)
                .IsUnique()
                .HasDatabaseName("UX_NewsletterSubscription_Email");

            modelBuilder.Entity<NewsletterSubscription>()
                .HasIndex(ns => ns.ConfirmationToken)
                .HasDatabaseName("IX_NewsletterSubscription_Token");

            modelBuilder.Entity<NewsletterSubscription>()
                .HasIndex(ns => new { ns.IsActive, ns.Frequency })
                .HasDatabaseName("IX_NewsletterSubscription_Active_Frequency");
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
        public DbSet<FinancialMovement> FinancialMovements { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Scholarship> Scholarships { get; set; }
        public DbSet<HomeContent> HomeContents { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
    }
}
