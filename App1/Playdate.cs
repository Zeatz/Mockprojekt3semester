using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class Playdate
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
        

        public Playdate(DateTime dt, string des)
        {
            Description = des;
            Date = dt;
        }
    }
}
