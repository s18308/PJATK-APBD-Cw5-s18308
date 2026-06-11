namespace PJATK_APBD_Cw5_s18308.DTOs;

public class PatientResponse
{
    public string Pesel { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Sex { get; set; } = null!;
    public List<PatientAdmissionResponse> Admissions { get; set; } = new();
    public List<PatientBedAssignmentResponse> BedAssignments { get; set; } = new();
}

public class PatientAdmissionResponse
{
    public int Id { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public WardResponse Ward { get; set; } = null!;
}

public class PatientBedAssignmentResponse
{
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public BedResponse Bed { get; set; } = null!;
}

public class BedResponse
{
    public int Id { get; set; }
    public BedTypeResponse BedType { get; set; } = null!;
    public RoomResponse Room { get; set; } = null!;
}

public class BedTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class RoomResponse
{
    public string Id { get; set; } = null!;
    public bool HasTv { get; set; }
    public WardResponse Ward { get; set; } = null!;
}

public class WardResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
