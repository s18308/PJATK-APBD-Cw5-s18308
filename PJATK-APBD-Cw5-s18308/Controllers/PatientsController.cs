using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_Cw5_s18308.DTOs;
using PJATK_APBD_Cw5_s18308.Exceptions;
using PJATK_APBD_Cw5_s18308.Services;

namespace PJATK_APBD_Cw5_s18308.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController(IDbService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await service.GetPatientsAsync(search, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBed(
        [FromRoute] string pesel,
        [FromBody] BedAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.AssignBedAsync(pesel, request, cancellationToken);
            return CreatedAtAction(nameof(GetPatients), new { search = pesel }, result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
