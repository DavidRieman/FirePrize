using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirePrize
{
    public class PrizePool : PropertyObservable
    {
        public PrizePool()
        {
            this.ID = Guid.NewGuid();
        }

        public PrizePool(string name)
            : this()
        {
            this.Name = name;
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        private Guid id;
        public Guid ID
        {
            get { return id; }
            set { SetProperty<Guid>(ref id, value); }
        }

        private FireCollection<Prize> prizes;
        [JsonIgnore]
        public FireCollection<Prize> Prizes
        {
            get { return prizes; }
            set { SetProperty<FireCollection<Prize>>(ref prizes, value); }
        }
    }
}