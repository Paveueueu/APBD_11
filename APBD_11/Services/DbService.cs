using APBD_11.DTOs;
using APBD_11.Data;
using APBD_11.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_11.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;
    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddPrescription(int idDoctor, PrescriptionDto dto)
    {
        if (dto.Medicaments == null || dto.Medicaments.Count < 1 || dto.Medicaments.Count > 10)
            throw new ArgumentException("Prescription must have [1-10] medicaments");
        if (dto.DueDate < dto.Date)
            throw new ArgumentException("Prescription.DueDate cannot be earlier than Prescription.Date");
        
        var doctor = await _context.Doctors.FindAsync(idDoctor);
        if (doctor == null)
            throw new ArgumentException("Doctor does not exist");
        
        var medicamentIds = dto.Medicaments.Select(m => m.IdMedicament).ToList();
        var existingMedicaments = await _context.Medicaments
            .Where(m => medicamentIds.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();
        if (existingMedicaments.Count != medicamentIds.Count)
            throw new ArgumentException("Non-existent medicament(s)");
        
        
        
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => 
                p.FirstName == dto.Patient.FirstName &&
                p.LastName == dto.Patient.LastName &&
                p.BirthDate == dto.Patient.BirthDate);
        
        
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try 
        {
            // Create a patient if he doesn't exist
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = dto.Patient.FirstName,
                    LastName = dto.Patient.LastName,
                    BirthDate = dto.Patient.BirthDate,
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
        
            // Create prescription
            var prescription = new Prescription
            {
                Date = dto.Date,
                DueDate = dto.DueDate,
                IdDoctor = idDoctor,
                IdPatient = dto.Patient.IdPatient,
                Patient = patient,
                Doctor = doctor,
                PrescriptionMedicaments = dto.Medicaments.Select(m => new PrescriptionMedicament
                {
                    IdMedicament = m.IdMedicament,
                    Dose = m.Dose,
                    Details = m.Description
                }).ToList()
            };
            
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            
            foreach (var medicamentDto in dto.Medicaments)
            {
                var pm = new PrescriptionMedicament
                {
                    IdPrescription = prescription.IdPrescription,
                    IdMedicament = medicamentDto.IdMedicament,
                    Dose = medicamentDto.Dose,
                    Details = medicamentDto.Description
                };
                _context.PrescriptionMedicaments.Add(pm);
            }
            
            await transaction.CommitAsync();
        }
        catch 
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<PatientDataDto?> GetPatientData(int id)
    {
        var result = await _context.Patients
            .Where(p => p.IdPatient == id)
            .Select(p => new PatientDataDto
            {
                IdPatient = p.IdPatient,
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthDate = p.BirthDate,
                Prescriptions = p.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionDataDto
                    {
                        IdPrescription = pr.IdPrescription,
                        Date = pr.Date,
                        DueDate = pr.DueDate,
                        Medicaments = pr.PrescriptionMedicaments.Select(pm => new MedicamentDto
                        {
                            IdMedicament = pm.Medicament.IdMedicament,
                            Name = pm.Medicament.Name,
                            Dose = pm.Dose,
                            Description = pm.Details
                        }).ToList(),
                        Doctor = new DoctorDto
                        {
                            IdDoctor = pr.Doctor.IdDoctor,
                            FirstName = pr.Doctor.FirstName,
                            LastName = pr.Doctor.LastName,
                            Email = pr.Doctor.Email
                        }
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        return result;
    }

}