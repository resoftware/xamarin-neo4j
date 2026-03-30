//
// VisualizationViewModel.cs
//
// Trevi Awater
// 10-01-2022
//
// © Xamarin.Neo4j
//

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Xamarin.Neo4j.Annotations;
using Xamarin.Neo4j.Models;

namespace Xamarin.Neo4j.ViewModels
{
    public class GraphViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private HtmlWebViewSource _source;

        public GraphViewModel(INavigation navigation, string html) : base(navigation)
        {
            Source = new HtmlWebViewSource
            {
                Html = html
            };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Bindable Properties

        public HtmlWebViewSource Source
        {
            get => _source;

            set
            {
                _source = value;

                OnPropertyChanged();
            }
        }

        #endregion
    }
}
