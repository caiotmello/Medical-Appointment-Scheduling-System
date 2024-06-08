using MedicalSystem.Domain.Entities;
using MedicalSystem.Domain.Enumerations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystem.Infrastructure.Persistence
{
    public class IdentityDataContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Patient> patients;
        public DbSet<Doctor> doctors;

        public IdentityDataContext()
        {
        }
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Doctor>(entity => { entity.ToTable("Doctor"); });
            builder.Entity<Patient>(entity => { entity.ToTable("Patient"); });

            builder.Entity<ApplicationUser>().HasQueryFilter(u => u.Status != UserStatusEnum.Inactive);
        }

    }
}
