using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FirePrize
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        private IFirebaseClient firebase;

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
        }

        private async void NewPrizePool_Click(object sender, RoutedEventArgs e)
        {
            string name = this.newPrizePoolName.Text;
            if (!string.IsNullOrWhiteSpace(name))
            {
                await firebase.PushAsync("prizePools", new PrizePool(name));
            }
        }
    }
}