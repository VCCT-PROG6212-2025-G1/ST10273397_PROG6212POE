using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;

namespace PROG6212POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClaimModel> ClaimModel { get; set; }

        public DbSet<UserModel> UserModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {

                entity.ToTable("UserModel");
                entity.HasKey(p => p.UserId).HasName("PK_User");

                entity.Property(p => p.UserId)
                .HasColumnName("UserId")
                .HasColumnType("int").ValueGeneratedNever();

                entity.Property(p => p.FirstName)
                .HasColumnName("FirstName");

                entity.Property(p => p.LastName)
                .HasColumnName("LastName");

                entity.Property(p => p.Email)
                .HasColumnName("Email");

                entity.Property(p => p.Password)
                .HasColumnName("Password");

                entity.Property(p => p.UserRole)
                .HasColumnName("Role");

                entity.Property(p => p.HourlyRate)
                .HasColumnName("HourlyRate")
                .HasColumnType("int");

            });

            modelBuilder.Entity<ClaimModel>(entity =>
            {
                entity.ToTable("ClaimModel");
                entity.HasKey(p => p.ClaimId).HasName("PK_Claim");

                entity.Property(p => p.ClaimId)
                .HasColumnName("ClaimId")
                .HasColumnType("int").ValueGeneratedNever();

                entity.Property(p => p.Title)
                .HasColumnName("Title");

                entity.Property(p => p.HoursWorked)
                .HasColumnName("HoursWorked")
                .HasColumnType("int");

                entity.Property(p => p.HourlyRate)
                .HasColumnName("HourlyRate")
                .HasColumnType("float");

                entity.Property(p => p.ClaimStatus)
                .HasColumnName("ClaimStatus");

                entity.Property(p => p.AdditionalNotes)
                .HasColumnName("AdditionalNotes");

                entity.Property(p => p.SuppDocName)
                .HasColumnName("SuppDocName");

                entity.Property(p => p.SuppDocPath)
                .HasColumnName("SuppDocPath");
            });
        }

    }
}
