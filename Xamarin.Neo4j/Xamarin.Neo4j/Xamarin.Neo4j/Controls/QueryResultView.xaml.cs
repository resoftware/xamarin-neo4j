using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Neo4j.Driver;
using Xamarin.Neo4j.Managers;
using Xamarin.Neo4j.Services;
using Xamarin.Neo4j.Models;
using Xamarin.Neo4j.Pages;
using Xamarin.Neo4j.Utilities;
using Query = Xamarin.Neo4j.Models.Query;

namespace Xamarin.Neo4j.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QueryResultView : ContentView
    {
        private QueryResult QueryResult => (QueryResult) BindingContext;

        public event EventHandler<GenericEventArgs<QueryResult>> CloseRequested;

        private string _neovisHtml;

        public QueryResultView()
        {
            InitializeComponent();
            BindingContextChanged += OnBindingContextChanged;
            graphView.Navigating += OnGraphViewNavigating;
        }

        private async void OnGraphViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            Console.WriteLine($"[Graph] Inline Navigating: {e.Url}");

            if (!e.Url.Contains("expand") || !e.Url.Contains("nodeId")) return;

            e.Cancel = true;

            var connectionString = ConnectionStringManager.ActiveConnectionString;
            if (connectionString == null) return;

            try
            {
                var neo4jService = IPlatformApplication.Current.Services.GetRequiredService<Neo4jService>();

                var match = System.Text.RegularExpressions.Regex.Match(e.Url, @"nodeId=(\d+)");
                if (!match.Success || !long.TryParse(match.Groups[1].Value, out var nodeId)) return;

                var result = await neo4jService.ExpandNode(nodeId, connectionString);

                if (!result.Success || result.Results == null) return;

                var (nodesJson, edgesJson) = GraphDataHelper.BuildJson(result.Results, connectionString.Id);

                var js = $"window.addGraphData({nodesJson}, {edgesJson}, {nodeId})";
                await graphView.EvaluateJavaScriptAsync(js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Graph] Inline expand failed: {ex.Message}");
            }
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            if (BindingContext == null || graphView == null)
            {
                Console.WriteLine($"[Graph] OnBindingContextChanged skipped: BindingContext={BindingContext}, graphView={graphView}");
                return;
            }

            Console.WriteLine($"[Graph] OnBindingContextChanged fired, CanDisplayGraph={QueryResult?.CanDisplayGraph}");

            ParseNeovisHtmlSafe();

            Console.WriteLine($"[Graph] Setting graphView.Source, html length={_neovisHtml?.Length ?? 0}");

            graphView.Source = new HtmlWebViewSource { Html = _neovisHtml };
        }

        private void ParseNeovisHtml()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Xamarin.Neo4j.Visualization.visgraph.html";

            var available = string.Join("\n", assembly.GetManifestResourceNames());
            Console.WriteLine($"[Graph] Looking for: {resourceName}");
            Console.WriteLine($"[Graph] Available resources: {available.Replace("\n", ", ")}");

            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                _neovisHtml = $"<html><body style='background:#111;color:#f55;font-family:monospace;padding:16px'>" +
                              $"<b>Resource not found:</b><br>{resourceName}<br><br>" +
                              $"<b>Available:</b><br>{available.Replace("\n", "<br>")}</body></html>";
                return;
            }

            using (stream)
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                var connectionId = ConnectionStringManager.ActiveConnectionString?.Id ?? Guid.Empty;

                var (nodesJson, edgesJson) = GraphDataHelper.BuildJson(QueryResult.Results, connectionId);

                var isDark = Application.Current.RequestedTheme == AppTheme.Dark;
                result = result.Replace("{{nodes}}", nodesJson);
                result = result.Replace("{{edges}}", edgesJson);
                result = result.Replace("{{backgroundColor}}", isDark ? "#292C31" : "#FFFFFF");
                result = result.Replace("{{textColor}}", isDark ? "#FFFFFF" : "#000000");

                _neovisHtml = result;
            }
        }

        private void ParseNeovisHtmlSafe()
        {
            try
            {
                ParseNeovisHtml();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Graph] ParseNeovisHtml threw: {ex.GetType().Name}: {ex.Message}");
                _neovisHtml = $"<html><body style='background:#111;color:#f55;font-family:monospace;padding:16px'>" +
                              $"<b>{ex.GetType().Name}</b><br>{ex.Message}<br><br>{ex.StackTrace?.Replace("\n", "<br>")}</body></html>";
            }
        }

        private async void OpenNeovis(object sender, EventArgs e)
        {
            var neo4jService = IPlatformApplication.Current.Services.GetRequiredService<Neo4jService>();
            var connectionString = ConnectionStringManager.ActiveConnectionString;
            await Application.Current.MainPage.Navigation.PushAsync(new GraphPage(_neovisHtml, connectionString, neo4jService));
        }

        private async void SaveQuery(object sender, EventArgs e)
        {
            var name = await Application.Current.MainPage.DisplayPromptAsync("Save Query", "How should the query be called?");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var query = new Query()
                {
                    Id = Guid.NewGuid(),
                    QueryText = QueryResult.Query,
                    Name = name,
                };

                SavedQueryManager.AddSavedQuery(query);
            }
        }

        private void CloseResultView(object sender, EventArgs e)
        {
            CloseRequested?.Invoke(sender, new GenericEventArgs<QueryResult>(QueryResult));
        }

        private async void OpenTableView(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new TablePage(QueryResult));
        }
    }
}
