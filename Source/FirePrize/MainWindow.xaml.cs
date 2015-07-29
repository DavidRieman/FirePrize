using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FirePrize
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IFirebaseClient firebase;

        public FireCollection<PrizePool> PrizePools { get; set; }
        public List<FireCollection<Prize>> PrizeMap { get; set; }
        public FireCollection<Prize> SelectedPrizePoolPrizes { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MainWindow()
        {
            this.PrizeMap = new List<FireCollection<Prize>>();

            InitializeComponent();

            this.DataContext = this;

            // Using FirePrize_Scratch DB; You must create your own Firebase DB (free), and use it here.
            // TODO: Allow non-code reconfiguration, even for ClickOnce launchers.
            var config = new FirebaseConfig()
            {
                AuthSecret = "m9hRXQvwANuKWJZJEbYWypPmsJw6MVCVRvfL8R80",
                BasePath = "https://fiery-fire-2370.firebaseio.com/",
            };
            firebase = new FirebaseClient(config);

            this.PrizePools = new FireCollection<PrizePool>(firebase, "prizePools");
        }

        public static PrizePool lastTrackedPool;

        private void NewPrizePool_Click(object sender, RoutedEventArgs e)
        {
            string poolName = this.newPrizePoolName.Text;
            if (string.IsNullOrWhiteSpace(poolName))
            {
                MessageBox.Show("Must supply a prize pool name.");
            }
            else if (this.PrizePools.Where(p => poolName.Equals(p.Name, StringComparison.OrdinalIgnoreCase)).Any())
            {
                MessageBox.Show("A prize pool of that name already exists.");
            }
            else
            {
                lastTrackedPool = new PrizePool(poolName);
                PrizePools.Add(lastTrackedPool);

                // Also create a new FireCollection of associated Prizes.
                var prizeMapName = string.Format("prizePoolPrizes_{0}", poolName);
                var newPrizeCollection = new FireCollection<Prize>(this.firebase, prizeMapName);
                this.PrizeMap.Add(newPrizeCollection);
                // TEST
                newPrizeCollection.Add(new Prize() { Name = "Cool Prize" });
                newPrizeCollection.Add(new Prize() { Name = "Another Prize" });
            }
        }

        private void PrizePoolListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0 && e.AddedItems[0] is PrizePool)
            {
                var name = string.Format("prizePoolPrizes_{0}", (e.AddedItems[0] as PrizePool).Name);
                SelectedPrizePoolPrizes = PrizeMap.Where(p => p.Name == name).FirstOrDefault();
                PropertyChanged(sender, new PropertyChangedEventArgs("SelectedPrizePoolPrizes"));
            }
        }
    }
}