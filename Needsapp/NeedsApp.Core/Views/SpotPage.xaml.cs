using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeedsApp.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpotPage : ContentPage
    {
        public SpotPage()
        {
            InitializeComponent();
            BindingContext = new SpotPageViewModel();
        }
    }

    class SpotPageViewModel : INotifyPropertyChanged
    {

        public SpotPageViewModel()
        {
            
        }

       


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}