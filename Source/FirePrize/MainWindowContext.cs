using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirePrize
{
    public class MainWindowContext : PropertyObservable
    {
        private FireCollection<PrizePool> prizePools;
        public FireCollection<PrizePool> PrizePools
        {
            get { return prizePools; }
            set { SetProperty<FireCollection<PrizePool>>(ref prizePools, value); }
        }

        private PrizePool selectedPrizePool;
        public PrizePool SelectedPrizePool
        {
            get { return selectedPrizePool; }
            set { SetProperty<PrizePool>(ref selectedPrizePool, value); }
        }
    }
}
