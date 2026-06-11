using Microsoft.EntityFrameworkCore;
using PJATK_APBD_Cw5_s18308.DTOs;
using PJATK_APBD_Cw5_s18308.Entities;
using PJATK_APBD_Cw5_s18308.Exceptions;
using PJATK_APBD_Cw5_s18308.Infrastructure;

namespace PJATK_APBD_Cw5_s18308.Services;

public class DbService(DatabaseContext ctx) : IDbService
{
    public async Task<ICollection<PatientResponse>> GetPatientsAsync(string? search, CancellationToken cancellationToken)
    {
        var query = ctx.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, pattern) ||
                EF.Functions.Like(p.LastName, pattern));
        }

        return await query
            .Select(p => new PatientResponse
            {
                Pesel = p.Pesel,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Sex = p.Sex ? "Male" : "Female",
                Admissions = p.Admissions.Select(a => new PatientAdmissionResponse
                {
                    Id = a.Id,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Ward = new WardResponse
                    {
                        Id = a.Ward.Id,
                        Name = a.Ward.Name,
                        Description = a.Ward.Description
                    }
                }).ToList(),
                BedAssignments = p.BedAssignments.Select(ba => new PatientBedAssignmentResponse
                {
                    Id = ba.Id,
                    From = ba.From,
                    To = ba.To,
                    Bed = new BedResponse
                    {
                        Id = ba.Bed.Id,
                        BedType = new BedTypeResponse
                        {
                            Id = ba.Bed.BedType.Id,
                            Name = ba.Bed.BedType.Name,
                            Description = ba.Bed.BedType.Description
                        },
                        Room = new RoomResponse
                        {
                            Id = ba.Bed.Room.Id,
                            HasTv = ba.Bed.Room.HasTv,
                            Ward = new WardResponse
                            {
                                Id = ba.Bed.Room.Ward.Id,
                                Name = ba.Bed.Room.Ward.Name,
                                Description = ba.Bed.Room.Ward.Description
                            }
                        }
                    }
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<BedAssignmentResponse> AssignBedAsync(string pesel, BedAssignmentRequest request, CancellationToken cancellationToken)
    {
        var patientExists = await ctx.Patients.AnyAsync(p => p.Pesel == pesel, cancellationToken);
        if (!patientExists)
            throw new NotFoundException($"Patient with PESEL '{pesel}' not found.");

        var ward = await ctx.Wards.FirstOrDefaultAsync(w => w.Name == request.Ward, cancellationToken);
        if (ward is null)
            throw new NotFoundException($"Ward '{request.Ward}' not found.");

        var bedType = await ctx.BedTypes.FirstOrDefaultAsync(bt => bt.Name == request.BedType, cancellationToken);
        if (bedType is null)
            throw new NotFoundException($"Bed type '{request.BedType}' not found.");

        var bedsOfTypeInWard = ctx.Beds.Where(b =>
            b.BedTypeId == bedType.Id &&
            b.Room.WardId == ward.Id);

        if (!await bedsOfTypeInWard.AnyAsync(cancellationToken))
            throw new NotFoundException(
                $"There are no beds of type '{request.BedType}' in ward '{request.Ward}'.");

        IQueryable<Bed> availableBeds;
        if (request.To.HasValue)
        {
            var requestTo = request.To.Value;
            availableBeds = bedsOfTypeInWard.Where(b => !b.BedAssignments.Any(ba =>
                ba.From < requestTo &&
                (ba.To == null || ba.To > request.From)));
        }
        else
        {
            availableBeds = bedsOfTypeInWard.Where(b => !b.BedAssignments.Any(ba =>
                ba.To == null || ba.To > request.From));
        }

        var availableBed = await availableBeds
            .OrderBy(b => b.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (availableBed is null)
            throw new NotFoundException(
                $"No available bed of type '{request.BedType}' in ward '{request.Ward}' for the requested period.");

        var assignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = availableBed.Id,
            From = request.From,
            To = request.To
        };

        await ctx.BedAssignments.AddAsync(assignment, cancellationToken);
        await ctx.SaveChangesAsync(cancellationToken);

        return new BedAssignmentResponse
        {
            Id = assignment.Id,
            PatientPesel = assignment.PatientPesel,
            BedId = assignment.BedId,
            From = assignment.From,
            To = assignment.To,
            BedType = bedType.Name,
            Ward = ward.Name,
            RoomId = availableBed.RoomId
        };
    }
}
