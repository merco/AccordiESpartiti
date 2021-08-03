using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChitarrApp.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

protected async override void OnAppearing()
        {
            base.OnAppearing();
            Aind.IsVisible = true;
            var b = await Uty.AutoSetup();

            var cnt = await Uty.Database.GetSoungCount();
            var cnt2 = await Uty.Database.GetAuthorsCount();
            LabelCount.Text = cnt2.ToString();
            LabelCount2.Text = cnt.ToString();
            MainLW.ItemsSource = Uty.SongPref.LastREV();
            Aind.IsVisible = false;

            if (Uty.RandomPlays==null)
            {
                Uty.RandomPlays = await Uty.Database.GetRandomCanzoni();
            }
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



        private void TapGestureRecognizerAutori_Tapped(object sender, EventArgs e)
        {
            Uty.SearchType = "A";
            AppShell SS = (AppShell)Shell.Current;
            SS.gotoCerca();
        }



        private async void TapGestureCanzoni_Tapped(object sender, EventArgs e)
        {


            //  await Shell.Current.GoToAsync ("ItemsPage?Q=A");

            Uty.SearchType = "C";
            AppShell SS = (AppShell)Shell.Current;
            SS.gotoCerca();
        }



        private void TapGestureRecognizerImage_Tapped(object sender, EventArgs e)
        {
           try
            {
            
                Xamarin.Essentials.Launcher.TryOpenAsync(new Uri("mailto:d.mercanti@gmail.com?subject=" + this.Title));
            } catch (Exception e1)
            {
                DisplayAlert("Mail Error", e1.Message,"OK");
            }

        }
    }
} 