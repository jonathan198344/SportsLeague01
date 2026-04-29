namespace SportsLeague.Domain.Interfaces.Repositories
{
    namespace SportsLeague.Domain.Interfaces.Repositories
    {
        public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
        {
            Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
            Task<IEnumerable<TournamentSponsor>> GetByTournamentIdAsync(int tournamentId);
            Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);
        }
    }
}
