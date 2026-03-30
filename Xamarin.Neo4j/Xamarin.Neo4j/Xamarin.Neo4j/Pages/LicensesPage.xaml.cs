using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Xamarin.Neo4j.ViewModels;

namespace Xamarin.Neo4j.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LicensesPage : ContentPage
    {
        public LicensesPage()
        {
            InitializeComponent();

            BindingContext = new LicensesViewModel(Navigation);
        }
    }
}
