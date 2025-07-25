using Api.Abstractions.Application;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos.Volunteer;
using Shared.Models;
namespace Api.Services.Application
{
    public class VolunteerRequestService : IVolunteerRequestService
    {
        private readonly IVolunteerRequestRepository volunteerRequestRepository;
        public VolunteerRequestService(IVolunteerRequestRepository volunteerRequestRepository)
        {
            this.volunteerRequestRepository = volunteerRequestRepository;
        }
        public async Task<List<VolunteerRequestDto>> GetAllByVolunteerIDAsync(int VolunteerId)
        {
            var requests = await volunteerRequestRepository.GetRequestsByVolunteerID(VolunteerId);
            return requests.Select(r => new VolunteerRequestDto
            {
                Id = r.Id,
                VolunteerId = r.VolunteerId,
                VolunteerName = $"{r.Volunteer?.Nombre} {r.Volunteer?.Apellidos}",
                ApproverId = r.ApproverId,
                ApproverName = r.Approver != null ? $"{r.Approver?.Nombre} {r.Approver?.Apellidos}" : null,
                CreatedAt = r.CreatedAt,
                State = r.State,
                Institution = r.Institution,
                Profession = r.Profession,
                Description = r.Description,
                Hours = r.Hours
            }).ToList();

        }
        public async Task<Result> CreateAsync(VolunteerRequestDto requestDto)
        {
            var ativeRequest = await volunteerRequestRepository.GetActiveRequest(requestDto.VolunteerId);
            if (ativeRequest != null)
            {
                return Result.Failure("Usted ya tiene una solicitud en proceso");
            }
            var request = new VolunteerRequest
            {
                VolunteerId = requestDto.VolunteerId,
                Institution = requestDto.Institution,
                Profession = requestDto.Profession,
                Description = requestDto.Description,
                Hours = requestDto.Hours,
                State = Shared.Enums.VolunteerState.Pending
            };
            await volunteerRequestRepository.CreateRequest(request);
            return Result.Success();
        }
    }
}
