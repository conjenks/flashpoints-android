using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashPoints.Models
{
    public class Prize
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int PointPrice { get; set; }
        public string ImagePath { get; set; }
        public int CurrentInventory { get; set; }
        public string Location { get; set; }
        public float ActualCost { get; set; }

        public ICollection<PrizeRedeemed> PrizesRedeemed { get; set; }
    }
}
