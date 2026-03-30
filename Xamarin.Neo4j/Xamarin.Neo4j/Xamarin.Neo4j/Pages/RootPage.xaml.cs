using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Xamarin.Neo4j.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage : TabbedPage
    {
        public RootPage()
        {
            InitializeComponent();
        }

        protected override void OnCurrentPageChanged()
        {
            Title = CurrentPage.Title;

            base.OnCurrentPageChanged();
        }
    }
}
