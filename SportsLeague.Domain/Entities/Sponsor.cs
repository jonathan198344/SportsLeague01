namespace SportsLeague.Domain.Entities
{
    using global::SportsLeague.Domain.Enums.SportsLeague.Domain.Enums;
    using System.Collections.Generic;

    namespace SportsLeague.Domain.Entities
    {
        public class Sponsor : AuditBase
        {
            public string Name { get; set; } = string.Empty;
            public string ContactEmail { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? WebsiteUrl { get; set; }
            public SponsorCategory Category { get; set; }

            public ICollection<TournamentSponsor> TournamentSponsors { get; set; } = new List<TournamentSponsor>();
        }
    }
}
