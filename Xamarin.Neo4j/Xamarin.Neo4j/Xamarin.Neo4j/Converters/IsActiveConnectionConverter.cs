//
// IsActiveConnectionConverter.cs
//
// Trevi Awater
// 13-01-2022
//
// © Xamarin.Neo4j
//

using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Xamarin.Neo4j.Managers;
using Xamarin.Neo4j.Models;

namespace Xamarin.Neo4j.Converters
{
    public class IsActiveConnectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Neo4jConnectionString targetConnection)
                return targetConnection.Id == ConnectionStringManager.ActiveConnectionString?.Id;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
