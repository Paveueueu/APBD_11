using APBD_11.DTOs;

namespace APBD_11.Services;

public interface IDbService
{
    Task AddPrescription(int idDoctor, PrescriptionDto dto);
    Task<PatientDataDto?> GetPatientData(int id);
}