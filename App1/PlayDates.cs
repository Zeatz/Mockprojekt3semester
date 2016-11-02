using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class PlayDates
    {
        public static PlayDates GetInstance { get; set; } = new PlayDates();
        public List<Playdate> PlaydatesList { get; set; }
        private PlayDates()
        {
            PlaydatesList = new List<Playdate>()
            {
                new Playdate(new DateTime(2016,10,30), "Henrik leger"),
                new Playdate(new DateTime(2016,10,29), "Ole spiser ost"),
                new Playdate(new DateTime(2016,10,27), "Peter wanker"),
                new Playdate(new DateTime(2016,11,2), "Kasper er noobet"),
                new Playdate(new DateTime(2016,11,1), "Nicolai er down"),
                new Playdate(new DateTime(2016,11,4), "Tomas er en gud"),
                new Playdate(new DateTime(2016,11,2), "Miki LOL"),
                new Playdate(new DateTime(2016,10,30), "Niels er sød"),
            };
        }
    }
}
