//
// JsonConvertConverter.cs
//
// Trevi Awater
// 13-01-2022
//
// © Xamarin.Neo4j
//

using System;
using System.Globalization;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;

namespace Xamarin.Neo4j.Converters
{
    public class JsonConvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
