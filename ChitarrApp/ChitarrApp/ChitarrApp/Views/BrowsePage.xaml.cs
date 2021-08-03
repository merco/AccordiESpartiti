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
    public partial class BrowsePage : ContentPage
    {
        int livello = 0;
       bool apertodaRicerca = false;
        Models.Categorie currentCategoria = null;
        Models.Autori currentAutore = null;
        Models.Canzoni currentCanzone = null;
        public BrowsePage()
        {
            InitializeComponent();
        }
        private async Task<bool> qryLevel()
        {
            Aind.IsVisible = true;
            MainLW.ItemsSource = null;
            miniInfo.Text = "";
            miniInfo.IsVisible = false;
            if (livello==0)
            {
                btnPrev.IsEnabled = false;
                this.Title = "Archivio";
                var lista = await Uty.Database.GetCategorie();
                MainLW.ItemsSource = lista;
                
            }
            if (livello == 1)
            {
                btnPrev.IsEnabled = true;
                this.Title = currentCategoria.Nome;
                var lista = await Uty.Database.GetAutoriByCategoria(currentCategoria.ID);
                MainLW.ItemsSource = lista;
            }
            if (livello == 2)
            {
               
                btnPrev.IsEnabled = true;
                this.Title = currentAutore.Nome;
                miniInfo.IsVisible = true;
                currentAutore = await Uty.Database.GetAutore(currentAutore.ID);
                miniInfo.Text = currentAutore.UiDetailText;
                var lista = await Uty.Database.GetCanzoniByAutore(currentAutore.ID);
                MainLW.ItemsSource = lista;
            }
            Aind.IsVisible = false;

            if (apertodaRicerca) btnPrev.IsEnabled = false;
            return true;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            this.Title = "Archivio";

            if (Uty.CurrentAutore!=null)
            {
                currentCategoria = await Uty.Database.GetCategoria(Uty.CurrentAutore.IDCategoria);
                currentAutore = Uty.CurrentAutore;
                livello = 2;
                Uty.CurrentAutore = null;
               apertodaRicerca = true;
            }


            var b = await qryLevel();
        }

        private async  void MainLW_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (livello==0)
            {
                currentCategoria = (Models.Categorie)e.Item;
                currentAutore = null;
                livello = 1;
                await qryLevel();

                return;
            }
            if (livello == 1)
            {
                currentAutore = (Models.Autori)e.Item;
                livello = 2;
                await qryLevel();

                return;
            }
            if (livello == 2)
            {
                Aind.IsVisible = true;
                miniInfo.IsVisible = false;
                MainLW.ItemsSource = null;
                currentCanzone = (Models.Canzoni)e.Item;
                Uty.CurrentSong = currentCanzone;

                await Shell.Current.GoToAsync("SongPage");
                Aind.IsVisible = false;
                return;
            }
        }

        private async void btnPrev_Clicked(object sender, EventArgs e)
        {
            if (apertodaRicerca) return;
            livello = livello - 1;
            if (livello < 0) livello = 0;
            await qryLevel();
        }
    }
}