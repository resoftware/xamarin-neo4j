//
// ViewModelBase.cs
//
// Trevi Awater
// 13-11-2021
//
// © Xamarin.Neo4j
//

using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Xamarin.Neo4j.ViewModels
{
    public abstract class ViewModelBase
    {
        public INavigation Navigation { get; set; }

        public Dictionary<string, ICommand> Commands { get; private set; }

        protected ViewModelBase(INavigation navigation)
        {
            Navigation = navigation;

            Commands = new Dictionary<string, ICommand>();
        }
    }
}
