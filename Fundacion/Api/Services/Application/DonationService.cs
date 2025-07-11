using Api.Abstractions.Application;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Microsoft.Identity.Client;
using Shared.Dtos.Donations;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class DonationService : IDonationService
    {
        private readonly IDonationsRepository _donationsRepository;

        public DonationService(IDonationsRepository donationsRepository)
        {
            _donationsRepository = donationsRepository;
        }

        public async Task<Result> AddMonetaryDonationAsync(AddMonetaryDonationDto dto)
        {
            var donation = new Donation
            {
                IdentificacionNumber = dto.Identification,

                Type = DonationType.Monetaria,

                Name = dto.Name,

            };
            await _donationsRepository.AddDonationAsync(donation);
            var monetarydonation = new MonetaryDonation
            {
                DonationId = donation.Id,

                Ammount = dto.Amount,

                Currency = dto.Currency,
            };

            await _donationsRepository.AddDonationMonetary(monetarydonation);

            return Result.Success();
        }
        // ir a traer el listado del repositorio y listarlo en un nuevo dto
    }
}
