using Microsoft.AspNetCore.Mvc;
using APBD_11.DTOs;
using APBD_11.Services;


namespace APBD_11.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController : ControllerBase
{
    private readonly IDbService _dbService;

    public PrescriptionsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost("/api/doctors/{idDoctor}/prescriptions")]
    public async Task<IActionResult> AddPrescription(int idDoctor, [FromBody] PrescriptionDto dto)
    {
        try
        {
            await _dbService.AddPrescription(idDoctor, dto);
            return Created();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetPatientData(int id)
    {
        var result = await _dbService.GetPatientData(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
}