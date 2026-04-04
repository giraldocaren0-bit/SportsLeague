namespace SportsLeague.Domain.Entities

{

    public class TournamentSponsor : AuditBase
    {

        public int TournamentId { get; set; }

        public int SponsorId { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public decimal ContractAmount { get; set; }


        // Navigation Propertiespublic Tournament Tournament { get; set; } = null!;

        public Sponsor Sponsor { get; set; } = null!;
       public Tournament Tournament { get; set; } = null!;

    }

}