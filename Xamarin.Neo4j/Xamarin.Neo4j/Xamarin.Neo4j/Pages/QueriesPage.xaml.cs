using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Xamarin.Neo4j.Models;
using Xamarin.Neo4j.ViewModels;

namespace Xamarin.Neo4j.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QueriesPage : ContentPage
    {
        private QueriesViewModel ViewModel => (QueriesViewModel)BindingContext;

        public QueriesPage()
        {
            InitializeComponent();

            BindingContext = new QueriesViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            ViewModel.LoadQueries();

            base.OnAppearing();
        }

        private void StartSessionWithQuery(object sender, ItemTappedEventArgs e)
        {
            ViewModel.StartSession((Query)e.Item);
        }
    }
}
