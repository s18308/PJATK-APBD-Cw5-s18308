using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_Cw5_s18308.DTOs;

public class BedAssignmentRequest
{
    [Required]
    public DateTime From { get; set; }
    public DateTime? To { get; set; }

    [Required]
    public string BedType { get; set; } = null!;

    [Required]
    public string Ward { get; set; } = null!;
}
