//
// TableViewModel.cs
//
// Trevi Awater
// 13-01-2022
//
// © Xamarin.Neo4j
//

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Xamarin.Neo4j.Annotations;
using Xamarin.Neo4j.Models;

namespace Xamarin.Neo4j.ViewModels
{
    public class TableViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IEnumerable<List<KeyValuePair<string, object>>> _rows;

        private int _position;

        public TableViewModel(INavigation navigation, QueryResult queryResult) : base(navigation)
        {
            var headers = queryResult.Results.Keys;
            var transposed = Transpose(queryResult.Results.Select(a => a.Value.ToArray()).ToArray());

            Rows = transposed.Select(row => row.Select((t, i) => new KeyValuePair<string, object>(headers.ElementAt(i), t)).ToList()).ToList();
        }

        private static IEnumerable<T[]> Transpose<T>(T[][] arr)
        {
            var rowCount = arr.Length;
            var columnCount = arr[0].Length;
            var transposed = new T[columnCount][];

            if (rowCount == columnCount)
            {
                transposed = (T[][])arr.Clone();

                for (var i = 1; i < rowCount; i++)
                    for (var j = 0; j < i; j++)
                        (transposed[i][j], transposed[j][i]) = (transposed[j][i], transposed[i][j]);
            }

            else
                for (var column = 0; column < columnCount; column++)
                {
                    transposed[column] = new T[rowCount];

                    for (var row = 0; row < rowCount; row++)
                        transposed[column][row] = arr[row][column];
                }

            return transposed;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Bindable Properties

        public IEnumerable<List<KeyValuePair<string, object>>> Rows
        {
            get => _rows;

            set
            {
                _rows = value;

                OnPropertyChanged();
            }
        }

        public int Position
        {
            get => _position;

            set
            {
                _position = value;

                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title => $"{Position + 1}/{Rows.Count()}";

        #endregion
    }
}
