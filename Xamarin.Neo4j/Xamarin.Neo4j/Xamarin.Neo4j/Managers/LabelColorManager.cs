//
// LabelColorManager.cs
//
// Trevi Awater
// 30-03-2026
//
// © Xamarin.Neo4j
//

using System;
using System.Collections.Generic;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace Xamarin.Neo4j.Managers
{
    /// <summary>
    /// Persists label → color mappings per connection, so node labels keep
    /// consistent colors across queries (like the Neo4j Browser).
    /// </summary>
    public static class LabelColorManager
    {
        private const string PreferenceKeyPrefix = "label_colors_";

        // Neo4j Browser–style palette
        private static readonly string[] Palette =
        {
            "#4C8EDA", // blue
            "#D9534F", // red
            "#57A773", // green
            "#F0AD4E", // amber
            "#9B59B6", // purple
            "#E06CA0", // pink
            "#00B5AD", // teal
            "#DA7B3A", // orange
            "#6477B0", // slate blue
            "#C0CA33", // lime
            "#7E57C2", // deep purple
            "#26A69A", // cyan-teal
            "#EF5350", // coral
            "#5C6BC0", // indigo
            "#66BB6A", // light green
            "#FFA726", // deep amber
            "#AB47BC", // magenta
            "#29B6F6", // light blue
            "#EC407A", // hot pink
            "#8D6E63", // brown
        };

        private static Dictionary<string, string> _cache;
        private static Guid? _cachedConnectionId;

        public static string GetColor(Guid connectionId, string label)
        {
            var map = GetMap(connectionId);

            if (map.TryGetValue(label, out var color))
                return color;

            // Assign next unused palette color
            color = Palette[map.Count % Palette.Length];
            map[label] = color;
            SaveMap(connectionId, map);

            return color;
        }

        private static Dictionary<string, string> GetMap(Guid connectionId)
        {
            if (_cachedConnectionId == connectionId && _cache != null)
                return _cache;

            var key = PreferenceKeyPrefix + connectionId;
            var json = Preferences.Default.Get(key, (string)null);

            _cache = string.IsNullOrEmpty(json)
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                  ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _cachedConnectionId = connectionId;
            return _cache;
        }

        private static void SaveMap(Guid connectionId, Dictionary<string, string> map)
        {
            var key = PreferenceKeyPrefix + connectionId;
            var json = JsonConvert.SerializeObject(map);
            Preferences.Default.Set(key, json);
        }
    }
}
