using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChitarrApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrefPage : ContentPage
    {
        public PrefPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainLW.ItemsSource = null;
            Device.BeginInvokeOnMainThread(() =>
            {
                MainLW.ItemsSource = Uty.SongPref.Pref;
            });
        }

        private async void MainLW_ItemTapped(object sender, ItemTappedEventArgs e)
        {


            Aind.IsVisible = true;
            Models.Canzoni CC = (Models.Canzoni)e.Item;


            Uty.CurrentSong = CC;
            await Shell.Current.GoToAsync("SongPage");
            Aind.IsVisible = false;
            return;
        }
    }
}