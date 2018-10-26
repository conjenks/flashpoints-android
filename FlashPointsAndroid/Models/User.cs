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
    }
}
