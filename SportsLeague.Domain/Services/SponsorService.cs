
using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Repositories.SportsLeague.Domain.Interfaces;
using System.Net.Mail;

namespace SportsLeague.Domain.Interfaces.Services
{
    public class SponsorService : ISponsorService
    {

        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly Repositories.ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository,
            Repositories.ITournamentSponsorRepository tournamentSponsorRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            ValidateSponsor(sponsor);

            var exists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (exists)
            {
                throw new InvalidOperationException(
                    $"The sponsor with name '{sponsor.Name}' It already exists.");
            }

            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"The sponsor withID {id}He was not found.");
            }

            ValidateSponsor(sponsor);

            if (!string.Equals(existing.Name, sponsor.Name, StringComparison.OrdinalIgnoreCase))
            {
                var duplicated = await _sponsorRepository.GetByNameAsync(sponsor.Name);
                if (duplicated != null && duplicated.Id != id)
                {
                    throw new InvalidOperationException(
                        $"The sponsor with name '{sponsor.Name}' It already exists.");
                }
            }

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;

            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);
            if (!exists)
            {
                throw new KeyNotFoundException($"The sponsor with ID {id} He was not found");
            }

            _logger.LogInformation("The sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task<TournamentSponsor> LinkTournamentAsync(
            int sponsorId,
            int tournamentId,
            decimal contractAmount)
        {
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException($"The sponsor with ID {sponsorId} He was not found.");
            }

            var tournamentExists = await _tournamentRepository.ExistsAsync(tournamentId);
            if (!tournamentExists)
            {
                throw new KeyNotFoundException($"The tournament with ID {tournamentId} He was not found.");
            }

            if (contractAmount <= 0)
            {
                throw new InvalidOperationException("The contract amount must be greater than 0");
            }

            var existing = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (existing != null)
            {
                throw new InvalidOperationException(
                    $"The sponsor with ID {sponsorId} He is already linked to the tournament {tournamentId}");
            }

            var tournamentSponsor = new TournamentSponsor
            {
                SponsorId = sponsorId,
                TournamentId = tournamentId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Linking sponsor {SponsorId} to tournament {TournamentId}",
                sponsorId,
                tournamentId);

            await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);

            return (await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId))!;
        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException($"The sponsor with ID {sponsorId} He was not found.");
            }

            return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
        }

        public async Task UnlinkTournamentAsync(int sponsorId, int tournamentId)
        {
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException($"The sponsor with ID {sponsorId} He was not found.");
            }

            var tournamentExists = await _tournamentRepository.ExistsAsync(tournamentId);
            if (!tournamentExists)
            {
                throw new KeyNotFoundException($"The tournament with ID {tournamentId} He was not found.");
            }

            var existing = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (existing == null)
            {
                throw new KeyNotFoundException(
                    $"The sponsor with ID {sponsorId} It is not linked to the tournament {tournamentId}");
            }

            _logger.LogInformation(
                "Unlinking sponsor {SponsorId} from tournament {TournamentId}",
                sponsorId,
                tournamentId);
            await _tournamentSponsorRepository.DeleteAsync(existing.Id);
        }

        private static void ValidateSponsor(Sponsor sponsor)
        {
            if (string.IsNullOrWhiteSpace(sponsor.Name))
            {
                throw new InvalidOperationException("The sponsor's name is required");
            }

            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail))
            {
                throw new InvalidOperationException("The contact email address is required");
            }

            try
            {
                _ = new MailAddress(sponsor.ContactEmail);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("The email format is invalid.");
            }

        }

        Task<Entities.TournamentSponsor> ISponsorService.LinkTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Entities.TournamentSponsor>> ISponsorService.GetTournamentsBySponsorAsync(int sponsorId)
        {
            throw new NotImplementedException();
        }
    }
}



