using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GracePointeSecurity.Library;

namespace GracePointeSecurity.ManagementApp
{
    /// <summary>
    /// Interaction logic for LocateWindow.xaml
    /// </summary>
    public partial class LocateWindow : Window
    {
        private readonly ObservableCollection<CameraModel> _cameraModels = new();

        public LocateWindow()
        {
            InitializeComponent();
        }

        private void CheckType(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton?.Name == "ByDate")
            {
                DateGrid.Visibility = Visibility.Visible;
            }
        }

        private void GoButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cameraModels.Clear();
            if (StartDatePicker.SelectedDate is null)
            {
                return;
            }

            if (!Directory.Exists(State.CurrentState.SortedVideoFolder))
            {
                return;
            }

            var items = new List<string>();
            var start = StartDatePicker.SelectedDate.Value;
            var end = EndDatePicker.SelectedDate ?? start;
            foreach (var hourPath in GetHourPaths(start, end))
            {
                try
                {
                    var path = Path.Combine(
                        State.CurrentState.SortedVideoFolder,
                        hourPath);
                    Console.WriteLine(path);
                    items.AddRange(
                        Directory.EnumerateFiles(
                                path)
                            .Select(x => x.Replace(State.CurrentState.SortedVideoFolder, string.Empty)));
                }
                catch
                {
                    // Ignored.
                }
            }

            foreach (var path in items)
            {
                _cameraModels.Add(new CameraModel(path));
            }

            VideoGrid.DataContext = _cameraModels;
        }

        private IEnumerable<string> GetHourPaths(DateTime start, DateTime end) =>
            Enumerable.Range(0, (int)(end - start).TotalHours)
                .Select(x => start.AddHours(x))
                .Select(x => x.ToString("yyyy\\\\MM\\\\dd\\\\HH"));

        public sealed record CameraModel(
            string Path)
        {
            public string CameraName =>
                System.IO.Path.GetFileName(Path)
                    ?.Split('.')[0];

            public string Date =>
                System.IO.Path.GetFileName(Path)
                    ?.Split('.')[1];

            public string Time =>
                System.IO.Path.GetFileName(Path)
                    ?.Split('.')[1];
        }
    }
}