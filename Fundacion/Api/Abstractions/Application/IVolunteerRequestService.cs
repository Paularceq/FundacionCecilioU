using Shared.Dtos.Volunteer;
using Shared.Models;

namespace Api.Abstractions.Application
{
    public interface IVolunteerRequestService
    {
        Task<Result> CreateAsync(VolunteerRequestDto requestDto);
        Task<List<VolunteerRequestDto>> GetAllByVolunteerIDAsync(int VolunteerId);
    }
}