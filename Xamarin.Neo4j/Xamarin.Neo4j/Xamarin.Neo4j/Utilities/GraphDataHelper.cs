//
// GraphDataHelper.cs
//
// Trevi Awater
// 30-03-2026
//
// © Xamarin.Neo4j
//

using System;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver;
using Xamarin.Neo4j.Managers;

namespace Xamarin.Neo4j.Utilities
{
    public static class GraphDataHelper
    {
        /// <summary>
        /// Extracts INode and IRelationship objects from a query result dictionary and
        /// builds JSON arrays suitable for the visgraph.html canvas renderer.
        /// </summary>
        public static (string NodesJson, string EdgesJson) BuildJson(
            Dictionary<string, List<object>> results,
            Guid connectionId)
        {
            var nodeDict = new Dictionary<long, INode>();
            var relationships = new List<IRelationship>();

            foreach (var values in results.Values)
            {
                foreach (var obj in values)
                {
                    switch (obj)
                    {
                        case INode node:
                            nodeDict[node.Id] = node;
                            break;
                        case IRelationship rel:
                            relationships.Add(rel);
                            break;
                    }
                }
            }

            var nodesJson = "[" + string.Join(",", nodeDict.Values.Select(n =>
            {
                var label = n.Labels.FirstOrDefault() ?? "Node";
                var title = string.Join(", ", n.Properties.Select(p => $"{p.Key}: {p.Value}"));
                var color = LabelColorManager.GetColor(connectionId, label);
                return $"{{\"id\":{n.Id},\"label\":\"{EscapeJs(label)}\",\"title\":\"{EscapeJs(title)}\",\"color\":\"{color}\"}}";
            })) + "]";

            var edgesJson = "[" + string.Join(",", relationships.Select(r =>
                $"{{\"from\":{r.StartNodeId},\"to\":{r.EndNodeId},\"label\":\"{EscapeJs(r.Type)}\"}}")) + "]";

            return (nodesJson, edgesJson);
        }

        public static string EscapeJs(string s)
        {
            return s?.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "") ?? string.Empty;
        }
    }
}
