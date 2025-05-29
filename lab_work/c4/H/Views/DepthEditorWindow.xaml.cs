using System.Linq;
using System.Windows;
using Photo3DEditor.Models;
using Photo3DEditor.Services;

namespace Photo3DEditor.Views
{
    public partial class DepthEditorWindow : Window
    {
        private readonly DataService _dataService = App.DataService;

        public DepthEditorWindow()
        {
            InitializeComponent();
            this.DataContext = _dataService;
            PointsList.ItemsSource = _dataService.Points;
            this.Left = App.weigth - 10;
            this.Top = 0;
            this.Width = 1920 - App.weigth;
            this.Height = App.height / 2;

            _dataService.Points.CollectionChanged += (s, e) => UpdateDisplayInfo();
            foreach (var point in _dataService.Points)
            {
                point.PropertyChanged += (s, e) => UpdateDisplayInfo();
            }
        }
        public static void RecalculateTriangles()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is ImageWindow image)
                {
                    image.Triangulate_Click(null, null);
                }
            }
        }

        private void Update3D_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _dataService.Points.Count; i++)
            {
                _dataService.Points[i].Index = i;
            }
            PointsList.Items.Refresh();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is Viewer3DWindow viewer)
                {
                    viewer.Refresh();
                    break;
                }
                if (window is ImageWindow image)
                {
                    image.Refresh();
                    break;
                }
            }
        }

        private void UpdateDisplayInfo()
        {
            for (int i = 0; i < _dataService.Points.Count; i++)
            {
                _dataService.Points[i].Index = i;
            }

            PointsList.Items.Refresh();
        }
        private void DeletePoint_Click(object sender, RoutedEventArgs e)
        {
            if (PointsList.SelectedItem is Point3D selectedPoint)
            {
                _dataService.Points.Remove(selectedPoint);

                if (_dataService.SelectedPoint == selectedPoint)
                {
                    _dataService.SelectedPoint = null;
                }

                PointsList.Items.Refresh();
                RecalculateTriangles();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Viewer3DWindow viewer)
                    {
                        viewer.Refresh();
                    }

                    if (window is ImageWindow image)
                    {
                        image.Refresh();
                    }
                }
            }
        }
    }
}