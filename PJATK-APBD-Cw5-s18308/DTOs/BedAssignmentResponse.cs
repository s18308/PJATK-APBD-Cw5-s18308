namespace PJATK_APBD_Cw5_s18308.DTOs;

public class BedAssignmentResponse
{
    public int Id { get; set; }
    public string PatientPesel { get; set; } = null!;
    public int BedId { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public string BedType { get; set; } = null!;
    public string Ward { get; set; } = null!;
    public string RoomId { get; set; } = null!;
}
