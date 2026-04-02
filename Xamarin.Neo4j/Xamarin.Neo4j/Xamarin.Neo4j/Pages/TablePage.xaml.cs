using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Newtonsoft.Json;
using Xamarin.Neo4j.Models;

namespace Xamarin.Neo4j.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TablePage : ContentPage
    {
        public TablePage(QueryResult queryResult)
        {
            InitializeComponent();
            LoadJson(queryResult);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.ThemeChanged += OnThemeChanged;
        }

        protected override void OnDisappearing()
        {
            App.ThemeChanged -= OnThemeChanged;
            base.OnDisappearing();
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            var isDark = Application.Current.RequestedTheme == AppTheme.Dark;
            jsonWebView.EvaluateJavaScriptAsync($"setTheme({(isDark ? "true" : "false")})");
        }

        private void LoadJson(QueryResult queryResult)
        {
            // Transpose column-oriented dict into a list of row objects
            var results = queryResult.Results;
            string json;
            if (results == null || results.Count == 0)
            {
                json = "[]";
            }
            else
            {
                var columns = results.Keys.ToList();
                var rowCount = results[columns[0]].Count;
                var rows = Enumerable.Range(0, rowCount)
                    .Select(i => columns.ToDictionary(c => c, c => results[c][i]))
                    .ToList();
                json = JsonConvert.SerializeObject(rows, Formatting.None);
            }

            var html = LoadTemplate(json);
            jsonWebView.Source = new HtmlWebViewSource { Html = html };
        }

        private static string LoadTemplate(string json)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "Xamarin.Neo4j.Visualization.jsonview.html";
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return $"<html><body>Resource not found: {resourceName}</body></html>";

            string html;
            using (stream)
            using (var reader = new StreamReader(stream))
                html = reader.ReadToEnd();

            var isDark = Application.Current.RequestedTheme == AppTheme.Dark;
            html = html.Replace("{{json}}", json);
            html = html.Replace("{{backgroundColor}}", isDark ? "#0c0c0c" : "#f5f5f5");
            html = html.Replace("{{textColor}}", isDark ? "#e2e2e2" : "#0c0c0c");
            html = html.Replace("{{toolbarBg}}", isDark ? "#141414" : "#ffffff");
            html = html.Replace("{{inputBg}}", isDark ? "#1a1a1a" : "#ffffff");
            html = html.Replace("{{borderColor}}", isDark ? "#2a2a2a" : "#e0e0e0");
            html = html.Replace("{{mutedColor}}", isDark ? "#868686" : "#868a92");
            html = html.Replace("{{stringColor}}", isDark ? "#8BC34A" : "#2E7D32");
            html = html.Replace("{{numberColor}}", isDark ? "#64B5F6" : "#1565C0");
            html = html.Replace("{{boolColor}}", isDark ? "#FFB74D" : "#E65100");
            html = html.Replace("{{nullColor}}", isDark ? "#868686" : "#868a92");
            html = html.Replace("{{keyColor}}", isDark ? "#e2e2e2" : "#0c0c0c");
            html = html.Replace("{{errorColor}}", isDark ? "#ff6b6b" : "#c62828");
            return html;
        }
    }
}
