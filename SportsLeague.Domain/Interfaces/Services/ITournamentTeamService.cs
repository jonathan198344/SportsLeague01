
using SportsLeague.Domain.Entities; 


namespace SportsLeague.Domain.Interfaces.Repositories;

    
    public interface ITournamentTeamService
    {
        Task<IEnumerable<TournamentTeam>> GetAllAsync();
        Task<TournamentTeam> GetByIdAsync(int id);

        // Asegúrate de que este nombre sea el mismo que usas en el servicio
        Task AddAsync(TournamentTeam entity);

        void Update(TournamentTeam entity);
        void Delete(TournamentTeam entity);

        // Los métodos de búsqueda que agregamos antes
        Task<TournamentTeam> GetByTournamentAndTeamAsync(int tournamentId, int teamId);
        Task<IEnumerable<TournamentTeam>> GetByTournamentAsync(int tournamentId);
    Task CreateAsync(TournamentTeam tournamentTeam);
}

