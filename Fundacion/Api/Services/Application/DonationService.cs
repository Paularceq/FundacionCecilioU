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

                Type = DonationType.Monetary,

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

        public async Task<Result<DonationDto>>GetDonationDetails(int id)
        {
            var donation = await _donationsRepository.GetDonationById(id);

            if (donation == null)
            {
                return Result<DonationDto>.Failure("Donation not found");
            }

            if(donation.Type == DonationType.Monetary)
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
            //hacer lo mismo para los otros tipos de donaciones
             return Result<DonationDto>.Failure("Donation type not supported");


        }
        }
    }


