using ChitarrApp.Services;
using ChitarrApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChitarrApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }
        
        protected async override void OnSleep()
        {
            var b = await Uty.SongPref.Save();
        }

        protected override void OnResume()
        {
        }
    }
}
