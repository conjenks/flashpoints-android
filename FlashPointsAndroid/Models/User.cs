using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlashPoints.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsAdmin { get; set; }

        public int Points { get; set; }
        public string EventsAttendedIDs { get; set; }
        public string PrizesRedeemedIDs { get; set; }

        public string EventsCreatedIDs { get; set; }

        public ICollection<PrizeRedeemed> PrizesRedeemed { get; set; }
        public ICollection<EventAttended> EventsAttended { get; set; }
    }

    public class PrizeRedeemed
    {
        public int UserID { get; set; }
        public User User { get; set; }
        public int PrizeID { get; set; }
        public Prize Prize { get; set; }
    }

    public class EventAttended
    {
        public int UserID { get; set; }
        public User User { get; set; }
        public int EventID { get; set; }
        public Event Event { get; set; }
    }
}
