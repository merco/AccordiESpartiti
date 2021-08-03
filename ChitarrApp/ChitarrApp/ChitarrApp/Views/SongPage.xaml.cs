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
    public partial class SongPage : ContentPage
    {
        Models.Song thisSONG = null;
        public SongPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //this.Title = Uty.CurrentSong.Nome;
            //this.info.Text = Uty.CurrentSong.Titolo;

            this.Title = Uty.CurrentSong.Titolo;

            Device.BeginInvokeOnMainThread(async () =>
            {
                thisSONG = await Uty.CurrentSong.GetSong();
                this.MainLW.ItemsSource = thisSONG.Rows;
            });

            Uty.SongPref.addLast(Uty.CurrentSong);


        }

        private  void btnPiu_Clicked(object sender, EventArgs e)
        {

            Device.BeginInvokeOnMainThread( () =>
            {

                if (thisSONG != null) thisSONG.Transpose(1);
                this.MainLW.ItemsSource = null;
                this.MainLW.ItemsSource = thisSONG.Rows;
            });

        }

        private void btnMeno_Clicked(object sender, EventArgs e)
        {
           

            Device.BeginInvokeOnMainThread( () =>
            {

                if (thisSONG != null) thisSONG.Transpose(-1);
                this.MainLW.ItemsSource = null;
                this.MainLW.ItemsSource = thisSONG.Rows;
            });
        }

        private void MainLW_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            Models.SongRow SR =(Models.SongRow) e.Item;
            var currentIDX=thisSONG.Rows.IndexOf(SR);
            var maxIDX = thisSONG.Rows.Count - 1;

            currentIDX = currentIDX + MainLW.VisibleItemCount-3;
            if (currentIDX > maxIDX) currentIDX = maxIDX;
            SR = thisSONG.Rows[currentIDX];
           

            MainLW.ScrollTo(SR, ScrollToPosition.MakeVisible, true); 
            

        }



        private void Cpiu_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Cpiu.IsEnabled = false;
                this.MainLW.ItemsSource = null;
                thisSONG.ChangeFont(+1);
                this.MainLW.ItemsSource = thisSONG.Rows;
                Cpiu.IsEnabled = true;
            });

            
        }

        private void Cmeno_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Cmeno.IsEnabled = false;
                this.MainLW.ItemsSource = null;
                thisSONG.ChangeFont(-1);
                this.MainLW.ItemsSource = thisSONG.Rows;
                Cmeno.IsEnabled = true;
            });
        }

        private void btnI_Clicked(object sender, EventArgs e)
        {
            menu.IsVisible = true;
            //if (Uty.SongPref.Pref.Contains(Uty.CurrentSong))
            if (Uty.SongPref.songContains(Uty.SongPref.Pref, Uty.CurrentSong))
            {
                btnPref.Text = "DIMENTICA";
            } else
            {
                btnPref.Text = "RICORDA";
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            menu.IsVisible = false;
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            if (btnPref.Text == "DIMENTICA")
            {
                Uty.SongPref.remPref(Uty.CurrentSong);
                menu.IsVisible = false;
                return;
            }

            Uty.SongPref.addPref(Uty.CurrentSong);
            menu.IsVisible = false;
        }



        private async void Button_Clicked_YT(object sender, EventArgs e)
        {
            string s = Uty.CurrentSong.Titolo + " " + Uty.CurrentSong.Nome;
            s = s.Replace( ' ', '+');

            //https://www.youtube.com/results?search_query=certe+notti+luciano+ligabue
            Uri U = new Uri("https://www.youtube.com/results?search_query=" + s);

            menu.IsVisible = false;
            await Xamarin.Essentials.Launcher.TryOpenAsync(U);

           


        }



        private void Button_Clicked_Send(object sender, EventArgs e)
        {

            menu.IsVisible = false;
            try
            {
                Services.SimplePDFWriter pdfWriter = new Services.SimplePDFWriter(842.0f, 1190.0f, 20.0f, 20.0f);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                var RR = thisSONG.Rows;
                //sb.AppendLine("---------------------------------");
                sb.AppendLine(" From ChitarrApp ! " + thisSONG.Name.Replace("TAB", "").ToUpper());
                // sb.AppendLine("-----------------------------");


                foreach (Models.SongRow R in RR)
                {
                    sb.AppendLine(R.ToString());
                }
                pdfWriter.Write(sb.ToString());
                string fn = Uty.GetPath(thisSONG.ID.ToString() + ".PDF");
                pdfWriter.Save(fn);

                Uty.SendLink(fn);
            } catch (Exception E1)
            {
                Shell.Current.DisplayAlert("Errore creazione PDF", E1.Message, "OK");
            }
           
        }

        private void btnA_Clicked(object sender, EventArgs e)
        {
            menu.IsVisible = false;
            String  tmpfile = Uty.GetPath("ProntuarioAccordi.pdf");
            try
            {
                
                if (System.IO.File.Exists(tmpfile)) System.IO.File.Delete(tmpfile);
                var ba = Uty.getFile("ProntuarioAccordi.pdf");
                System.IO.File.WriteAllBytes(tmpfile, ba);
            }
            catch (Exception E1)
            {
                Shell.Current.DisplayAlert("Errore creazione PDF", E1.Message, "OK");
            }

            if (System.IO.File.Exists(tmpfile))
            {



                Uty.OpenPDFLink(tmpfile);
            }
                

        }



        private async void Button_Clicked_mp3(object sender, EventArgs e)
        {
            bool retVal = false;
            string s = Uty.CurrentSong.Titolo + " " + Uty.CurrentSong.Nome;
            s = s.Replace(' ', '+');
            menu.IsVisible = false;
            var pathMp3 = Uty.GetDownloadPath(Uty.CurrentSong.Titolo + ".mp3");

            //Uty.OpenFolder(System.IO.Path.GetDirectoryName(pathMp3));


            if (System.IO.File.Exists(pathMp3))
            {
                retVal = await DisplayAlert("MP3", "Già presente.\r\n Lo vuoi riscaricare ?", "SI", "NO", FlowDirection.LeftToRight);

                if (!retVal)
                {
                    Uty.OpenPDFLink(pathMp3);
                    return;
                }
            }

            if (!retVal)
            {
                retVal = await DisplayAlert("MP3", "Lo vuoi Scaricare ?", "SI", "NO", FlowDirection.LeftToRight);
                if (!retVal) return;
            }


            Device.BeginInvokeOnMainThread(async () =>
            {
            
                Aind.IsVisible = true;
            });

           
            var ss = await Uty.DoMP3(s, Uty.CurrentSong.Titolo);

          
            Device.BeginInvokeOnMainThread(async () =>
            {
                Aind.IsVisible = false;
            });
            if (ss.StartsWith("@"))
            {
                await DisplayAlert("MP3", "FATTO !", "OK");
                Uty.OpenPDFLink(ss.Substring(1));
            }
            else
            {
                await DisplayAlert("MP3", ss, " Annulla ");
            }
        }
    }
}