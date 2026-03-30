//
// LicensesViewModel.cs
//
// Trevi Awater
// 15-03-2022
//
// © Xamarin.Neo4j
//

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Xamarin.Neo4j.Annotations;
using License = Xamarin.Neo4j.Models.License;

namespace Xamarin.Neo4j.ViewModels
{
    public class LicensesViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<License> _licenses;

        public LicensesViewModel(INavigation navigation) : base(navigation)
        {
            LoadLicenses();
        }

        private void LoadLicenses()
        {
            var assembly = GetType().GetTypeInfo().Assembly;

            string licenseFile = $"Xamarin.Neo4j.Resources.licenses.json";

            using (var stream = assembly.GetManifestResourceStream(licenseFile))
            using (var reader = new StreamReader(stream))
                Licenses = JsonConvert.DeserializeObject<List<License>>(reader.ReadToEnd());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Bindable Properties

        public List<License> Licenses
        {
            get => _licenses;

            set
            {
                _licenses = value;

                OnPropertyChanged();
            }
        }

        #endregion
    }
}
