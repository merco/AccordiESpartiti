using ChitarrApp.Models;
using ChitarrApp.ViewModels;
using ChitarrApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChitarrApp.Views
{

    [QueryProperty("Q", "Q")]
    public partial class ItemsPage : ContentPage
    {

        string _searchType = "";
        public ItemsPage()
        {
            InitializeComponent();

           
        }
        public string Q
        {
            set
            {
                _searchType = Uri.UnescapeDataString(value);
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Uty.CurrentAutore = null;
            Uty.CurrentSong = null;
            _searchType = Uty.SearchType;
            if (!string.IsNullOrEmpty(_searchType))
            {
                if (_searchType == "A") rba.IsChecked = true;
                if (_searchType == "C") rbc.IsChecked = true;

            }
        }

        private async void txtEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (rbc.IsChecked)
            {
                MainLW.ItemsSource = null;
                string testo = txtEdit.Text;
                if (testo.Length < 3) return;
                Aind.IsVisible = true;
                var cz = await Uty.Database.GetCanzoni(testo);
                MainLW.ItemsSource = cz;
                Aind.IsVisible = false;
            }
            if (rba.IsChecked)
            {
                MainLW.ItemsSource = null;
                string testo = txtEdit.Text;
                if (testo.Length <=0) return;
                Aind.IsVisible = true;
                var cz = await Uty.Database.GetAutori(testo);
                MainLW.ItemsSource = cz;
                Aind.IsVisible = false;
            }

        }

        private void rba_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            MainLW.ItemsSource = null;
            txtEdit.Text = "";

        }

        private async void MainLW_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (rba.IsChecked)
            {
                Aind.IsVisible = true;
                Autori AA = (Autori)e.Item;
                Uty.CurrentSong = null;
                Uty.CurrentAutore = AA;
                await Shell.Current.GoToAsync("BrowsePage");
                Aind.IsVisible = false;
                return;
            }

            if (rbc.IsChecked)
            {
                Aind.IsVisible = true;
                Canzoni CC = (Canzoni)e.Item;
                 
                Uty.CurrentAutore =await  Uty.Database.GetAutore(CC.IDAutore);
                Uty.CurrentSong = CC;
                await Shell.Current.GoToAsync("SongPage");
                Aind.IsVisible = false;
                return;
            }

        }
    }
}