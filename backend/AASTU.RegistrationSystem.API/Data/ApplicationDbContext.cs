using Microsoft.EntityFrameworkCore;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<RegistrationSlip> RegistrationSlips { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CostSharingForm> CostSharingForms { get; set; }
        public DbSet<GradeReport> GradeReports { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<RegistrationSlip>()
                .HasIndex(rs => rs.StudentID);

            modelBuilder.Entity<RegistrationSlip>()
                .HasIndex(rs => rs.SerialNumber)
                .IsUnique()
                .HasFilter("[SerialNumber] IS NOT NULL");

            // Configure decimal precision for GPA and CGPA
            modelBuilder.Entity<GradeReport>()
                .Property(g => g.GPA)
                .HasPrecision(5, 2);

            modelBuilder.Entity<GradeReport>()
                .Property(g => g.CGPA)
                .HasPrecision(5, 2);
        }
    }
}
