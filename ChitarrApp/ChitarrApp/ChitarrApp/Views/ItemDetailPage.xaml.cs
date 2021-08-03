using ChitarrApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace ChitarrApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}