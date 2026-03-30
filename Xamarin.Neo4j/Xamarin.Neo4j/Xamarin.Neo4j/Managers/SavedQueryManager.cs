//
// SavedQueryManager.cs
//
// Trevi Awater
// 11-01-2022
//
// © Xamarin.Neo4j
//

using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using Xamarin.Neo4j.Models;

namespace Xamarin.Neo4j.Managers
{
    public class SavedQueryManager
    {
        private const string SavedQueriesKey = "saved_queries";

        public static List<Query> GetSavedQueries()
        {
            var json = Preferences.Default.Get(SavedQueriesKey, (string)null);

            if (string.IsNullOrEmpty(json))
                return new List<Query>();

            return JsonConvert.DeserializeObject<List<Query>>(json);
        }

        public static void AddSavedQuery(Query query)
        {
            var savedQueries = GetSavedQueries();

            savedQueries.Add(query);

            SaveSavedQueries(savedQueries);
        }

        private static void SaveSavedQueries(IEnumerable<Query> savedQueries)
        {
            var json = JsonConvert.SerializeObject(savedQueries);

            Preferences.Default.Set(SavedQueriesKey, json);
        }

        public static void DeleteSavedQuery(Query query)
        {
            var savedQueries = GetSavedQueries();

            savedQueries = savedQueries.Where(cs => cs.Id != query.Id)
                .ToList();

            SaveSavedQueries(savedQueries);
        }
    }
}
