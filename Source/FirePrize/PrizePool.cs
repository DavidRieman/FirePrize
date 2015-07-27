using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirePrize
{
    public class PrizePool
    {
        public PrizePool()
        {
            this.Prizes = new List<Prize>();
            this.ID = Guid.NewGuid();
        }

        public PrizePool(string name)
            : this()
        {
            this.Name = name;
        }

        public string Name { get; set; }
        public Guid ID { get; set; }
        public List<Prize> Prizes { get; set; }
    }
}