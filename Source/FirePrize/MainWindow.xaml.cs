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

        public MainWindowContext Context { get { return this.DataContext as MainWindowContext; } }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MainWindow()
        {
            InitializeComponent();

            // Using FirePrize_Scratch DB; You must create your own Firebase DB (free), and use it here.
            // TODO: Allow non-code reconfiguration, even for ClickOnce launchers.
            var config = new FirebaseConfig()
            {
                AuthSecret = "m9hRXQvwANuKWJZJEbYWypPmsJw6MVCVRvfL8R80",
                BasePath = "https://fiery-fire-2370.firebaseio.com/",
            };
            firebase = new FirebaseClient(config);

            this.Context.PrizePools = new FireCollection<PrizePool>(firebase, "prizePools");
        }

        public static PrizePool lastTrackedPool;

        private void NewPrizePool_Click(object sender, RoutedEventArgs e)
        {
            string poolName = this.newPrizePoolName.Text;
            if (string.IsNullOrWhiteSpace(poolName))
            {
                MessageBox.Show("Must supply a prize pool name.");
            }
            else if (this.Context.PrizePools.Where(p => poolName.Equals(p.Name, StringComparison.OrdinalIgnoreCase)).Any())
            {
                MessageBox.Show("A prize pool of that name already exists.");
            }
            else
            {
                lastTrackedPool = new PrizePool(poolName);
                this.Context.PrizePools.Add(lastTrackedPool);
                
                // Also create a new FireCollection of associated Prizes.
                var prizesName = string.Format("prizePoolPrizes_{0}", poolName);
                lastTrackedPool.Prizes = new FireCollection<Prize>(this.firebase, prizesName);

                // TEST
                lastTrackedPool.Prizes.Add(new Prize() { Name = "Cool Prize" });
                lastTrackedPool.Prizes.Add(new Prize() { Name = "Another Prize" });
            }
        }

        private void PrizePoolListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0 && e.AddedItems[0] is PrizePool)
            {
                var pool = e.AddedItems[0] as PrizePool;
                this.Context.SelectedPrizePool = pool;
                if (pool.Prizes == null)
                {
                    var name = string.Format("prizePoolPrizes_{0}", pool.Name);
                    pool.Prizes = new FireCollection<Prize>(firebase, name);
                }
            }
        }

        private void PrizeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}