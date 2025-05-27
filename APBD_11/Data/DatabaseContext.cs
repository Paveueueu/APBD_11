using Microsoft.EntityFrameworkCore;
using APBD_11.Models;

namespace APBD_11.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                IdPatient = 1,
                FirstName = "Aa",
                LastName = "B",
                BirthDate = new DateTime(2000, 1, 1)
            },
            new Patient
            {
                IdPatient = 2,
                FirstName = "Cc",
                LastName = "D",
                BirthDate = new DateTime(2001, 2, 3)
            }
        );
        
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor
            {
                IdDoctor = 1,
                FirstName = "Dr",
                LastName = "A",
                Email = "drA@mail.com",
            },
            new Doctor
            {
                IdDoctor = 2,
                FirstName = "Dr",
                LastName = "B",
                Email = "drB@mail.com",
            }
        );
        
        modelBuilder.Entity<Medicament>().HasData(
            new Medicament
            {
                IdMedicament = 1,
                Description = "",
                Name = "Medicament 1",
                Type = "1"
            },
            new Medicament
            {
                IdMedicament = 2,
                Description = "",
                Name = "Medicament 2",
                Type = "2"
            }
        );
    }
}