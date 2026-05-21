using SportsLeague.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Entities
{
    public class Player : AuditBase

    {

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public int Number { get; set; }

        public PlayerPosition Position { get; set; }
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public int TeamId { get; set; } // Foreign Key
        public Team Team { get; set; } = null!;// Navigation Property
        public ICollection<MatchLineup> MatchLineups { get; set; } = new List<MatchLineup>();


    }
}
