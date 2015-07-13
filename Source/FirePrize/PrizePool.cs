using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirePrize
{
    public class PrizePool
    {
        public PrizePool(string name)
        {
            this.Name = name;
            this.Prizes = new List<Prize>();
        }

        public string Name { get; set; }
        public List<Prize> Prizes { get; set; }
    }
}