using Api.Abstractions.Application;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos.Donations;
using Shared.Enums;
using Shared.Models;

namespace Api.Services.Application
{
    public class DonationService : IDonationService
    {
        private readonly IDonationsRepository _donationsRepository;
        private readonly IFinancialRepository _financialRepository;

        public DonationService(
            IDonationsRepository donationsRepository,
            IFinancialRepository financialRepository)
        {
            _donationsRepository = donationsRepository;
            _financialRepository = financialRepository;
        }

        public async Task<Result> AddMonetaryDonationAsync(AddMonetaryDonationDto dto)
        {
            var donation = new Donation
            {
                IdentificacionNumber = dto.Identification,
                Type = DonationType.Monetary,
                Name = dto.Name,
            };
            await _donationsRepository.AddDonationAsync(donation);

            var monetaryDonation = new MonetaryDonation
            {
                DonationId = donation.Id,
                Ammount = dto.Amount,
                Currency = dto.Currency,
            };
            await _donationsRepository.AddDonationMonetary(monetaryDonation);

            // Registrar el movimiento financiero que acredita el monto al saldo
            var financialMovement = new FinancialMovement
            {
                Amount = (decimal)dto.Amount,
                Currency = dto.Currency,
                Type = MovementType.Inbound,
                Description = $"Donación monetaria recibida de {dto.Name}",
                Date = DateTime.Now,
                CreatedById = dto.CreatedById
            };
            await _financialRepository.AddFinancialMovementAsync(financialMovement);

            return Result.Success();
        }

        public async Task<IEnumerable<DonationDto>> GetAllDonationsAsync()
        {
            var donations = await _donationsRepository.GetAllDonationsAsync();
            return donations.Select(d => new DonationDto
            {
                Id = d.Id,
                Name = d.Name,
                IdentificationNumber = d.IdentificacionNumber,
                Type = d.Type,
            });
        }

        public async Task<Result<DonationDto>> GetDonationDetails(int id)
        {
            var donation = await _donationsRepository.GetDonationById(id);

            if (donation == null)
            {
                return Result<DonationDto>.Failure("Donation not found");
            }

            if (donation.Type == DonationType.Monetary)
            {
                var monetaryDonation = await _donationsRepository.GetMonetaryDonation(donation.Id);
                if (monetaryDonation == null)
                {
                    return Result<DonationDto>.Failure("Monetary donation details not found");
                }

                return Result<DonationDto>.Success(new DonationDto
                {
                    Id = donation.Id,
                    Name = donation.Name,
                    IdentificationNumber = donation.IdentificacionNumber,
                    Type = donation.Type,
                    Amount = monetaryDonation.Ammount,
                    Currency = monetaryDonation.Currency
                });
            }

            return Result<DonationDto>.Failure("Donation type not supported");
        }
    }
}


