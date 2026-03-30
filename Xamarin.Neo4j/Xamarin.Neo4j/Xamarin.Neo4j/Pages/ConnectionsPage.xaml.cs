using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Xamarin.Neo4j.Managers;
using Xamarin.Neo4j.Models;
using Xamarin.Neo4j.ViewModels;

namespace Xamarin.Neo4j.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionsPage : ContentPage
    {
        private ConnectionsViewModel ViewModel => (ConnectionsViewModel)BindingContext;

        public ConnectionsPage()
        {
            InitializeComponent();

            BindingContext = new ConnectionsViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            ViewModel.LoadConnectionStrings();

            base.OnAppearing();
        }

        private void SetActive(object sender, ItemTappedEventArgs e)
        {
            ViewModel.SetActiveConnectionString((Neo4jConnectionString)e.Item);
        }
    }
}
