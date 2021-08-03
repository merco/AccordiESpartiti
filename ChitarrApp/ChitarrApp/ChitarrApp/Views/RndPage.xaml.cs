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
    public partial class RndPage : ContentPage
    {
        public RndPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainLW.ItemsSource = null;
            this.Title = "JukeBox";
            Device.BeginInvokeOnMainThread(() =>
            {
                MainLW.ItemsSource = Uty.RandomPlays;
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

        private  void btnI_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Uty.RandomPlays = await Uty.Database.GetRandomCanzoni();
                MainLW.ItemsSource = Uty.RandomPlays;
            });
        }
    }
}