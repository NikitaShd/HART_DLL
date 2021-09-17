using Andriod_Hart.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Andriod_Hart.Views
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