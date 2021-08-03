using ChitarrApp.ViewModels;
using ChitarrApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace ChitarrApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public ShellContent CercaTab;
        public  AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(SongPage), typeof(SongPage));
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(BrowsePage), typeof(BrowsePage));
            Routing.RegisterRoute(nameof(RndPage), typeof(RndPage));
            Routing.RegisterRoute(nameof(PrefPage), typeof(PrefPage));
            Uty.LocalAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            CercaTab = CercaPage;
        }
        public void gotoCerca()
        {
            main_tab_bar.CurrentItem = CercaTab;
        }

    }
}
