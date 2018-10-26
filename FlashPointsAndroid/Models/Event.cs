using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashPoints.Models
{
    public class Event
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int PointValue { get; set; }
        public string QRcode { get; set; }
        public bool Approved { get; set; }
        public string Creator { get; set; }
        public string Location { get; set; }
        public int NumberAttended { get; set; }
    }
}
