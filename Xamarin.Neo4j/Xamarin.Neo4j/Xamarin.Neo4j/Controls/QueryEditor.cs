//
// QueryEditor.cs
//
// Trevi Awater
// 13-01-2022
//
// © Xamarin.Neo4j
//

using System;
using Microsoft.Maui.Controls;

namespace Xamarin.Neo4j.Controls
{
    public class QueryEditor : Editor
    {
        public double MaxHeight { get; set; }

        public event EventHandler ExecuteClicked;

        public void RaiseExecuteClicked()
        {
            ExecuteClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
