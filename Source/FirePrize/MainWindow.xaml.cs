using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FirePrize
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        private IFirebaseClient firebase;
        private LogicEqualityComparer<PrizePool> prizePoolComparer = new LogicEqualityComparer<PrizePool>();

        public ObservableCollection<PrizePool> PrizePools { get; set; }

        public MainWindow()
        {
            PrizePools = new ObservableCollection<PrizePool>();

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

            ValueRootAddedEventHandler<IEnumerable<PrizePool>> addedHandler = (s, newObjects) =>
            {
                // May be null upon startup if the Firebase DB has not yet been (re)created; just ignore as we 
                // will start getting updates when we actually add our first object, etc.
                if (newObjects == null)
                {
                    return;
                }

                // Locate any removals and additions which need to occur, before we have to move to the UI thread.
                // This will allow us to avoid clearing the list / making unnecessary jarring UI changes.
                var removals = this.PrizePools.Except(newObjects, prizePoolComparer).ToList();
                var additions = newObjects.Except(this.PrizePools, prizePoolComparer).Where(p => p != null).ToList();

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (PrizePool removal in removals)
                    {
                        this.PrizePools.Remove(removal);
                    }
                    foreach (PrizePool addition in additions)
                    {
                        this.PrizePools.Add(addition);
                    }
                }));
            };
            firebase.OnChangeGetAsync("prizePools", addedHandler);

            //firebase.OnAsync("prizePools", (s, a) =>
            //{
            //    Console.WriteLine("ADD!");
            //}, (s, a) =>
            //{
            //    Console.WriteLine("CHANGE!");
            //}, (s, a) =>
            //{
            //    Console.WriteLine("REMOVE!");
            //});
        }

        public static PrizePool lastTrackedPool;

        private async void NewPrizePool_Click(object sender, RoutedEventArgs e)
        {
            string name = this.newPrizePoolName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Must supply a prize pool name.");
            }
            else if (this.PrizePools.Where(p => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)).Any())
            {
                MessageBox.Show("A prize pool of that name already exists.");
            }
            else
            {
                lastTrackedPool = new PrizePool(name);
                lastTrackedPool.Prizes.Add(new Prize() { Name = "Cool Prize" });
                lastTrackedPool.Prizes.Add(new Prize() { Name = "Another Prize" });
                PrizePools.Add(lastTrackedPool);
                await this.firebase.SetAsync("prizePools", PrizePools);
            }
        }
    }
}