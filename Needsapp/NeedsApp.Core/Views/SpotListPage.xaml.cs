using NeedsApp.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeedsApp.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpotListPage : ContentPage
    {
        public SpotListPage()
        {
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = (SpotListViewModel)this.BindingContext;
            if (vm != null && vm.ViewSpotCommand != null && vm.ViewSpotCommand.CanExecute(null))
                vm.ViewSpotCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = (SpotListViewModel)this.BindingContext;
            if (vm != null && vm.RefreshDataCommand != null && vm.RefreshDataCommand.CanExecute(null))
                vm.RefreshDataCommand.Execute(null);
        }
    }
}