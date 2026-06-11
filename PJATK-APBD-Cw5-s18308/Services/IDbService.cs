using PJATK_APBD_Cw5_s18308.DTOs;

namespace PJATK_APBD_Cw5_s18308.Services;

public interface IDbService
{
    Task<ICollection<PatientResponse>> GetPatientsAsync(string? search, CancellationToken cancellationToken);
    Task<BedAssignmentResponse> AssignBedAsync(string pesel, BedAssignmentRequest request, CancellationToken cancellationToken);
}
