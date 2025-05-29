using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Photo3DEditor.Models;
using Photo3DEditor.Services;
using System.Numerics;
using System.IO;
using DelaunatorSharp;
using Point = System.Windows.Point;

using System.Windows.Media.Media3D;
using Point3D = Photo3DEditor.Models.Point3D;
using System.Collections.ObjectModel;

namespace Photo3DEditor.Views
{
    public partial class ImageWindow : Window
    {
        private readonly DataService _dataService = App.DataService;
        private ScaleTransform ImageScale = new ScaleTransform();
        private TranslateTransform ImageTranslate = new TranslateTransform();
        public static bool isFromObj = false;
        private Point _lastMousePos;
        private bool _isDragging = false;

        public ImageWindow()
        {
            InitializeComponent();
            DataContext = _dataService;
            this.Left = App.weigth - 10; // слева
            this.Top = 1080 - 600; // сверху
            this.Width = 600; // ширина
            this.Height = 600; // высота
        }
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Image files|*.jpg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true && MainImage is not null)
            {
                MainImage.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point canvasPos = e.GetPosition(ImageCanvas);

            GeneralTransform imageTransform = MainImage.TransformToAncestor(ImageCanvas);
            Point imageTopLeft = imageTransform.Transform(new Point(0, 0));

            double renderedWidth = MainImage.ActualWidth;
            double renderedHeight = MainImage.ActualHeight;

            if (canvasPos.X < imageTopLeft.X || canvasPos.X > imageTopLeft.X + renderedWidth ||
                canvasPos.Y < imageTopLeft.Y || canvasPos.Y > imageTopLeft.Y + renderedHeight)
                return; 

            double actualX = canvasPos.X;
            double actualY = canvasPos.Y;

            var newPoint = new Point3D { X = actualX, Y = actualY, Z = 0 };
            _dataService.Points.Add(newPoint);
            _dataService.SelectedPoint = newPoint;
            Triangulate_Click(sender, e);
        }
        private void Canvas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point canvasPos = e.GetPosition(ImageCanvas);
            SelectPoint(canvasPos);
        }
        private void SelectPoint(Point canvasPos)
        {
            const double clickRadius = 10;
            Point3D? closestPoint = null;
            double minDistance = double.MaxValue;

            foreach (var point in _dataService.Points)
            {
                double distance = Math.Sqrt(
                    Math.Pow(canvasPos.X - point.X, 2) +
                    Math.Pow(canvasPos.Y - point.Y, 2));

                if (distance < clickRadius && distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                    closestPoint.X += 30;
                    closestPoint.Y += 30;
                }
            }
            
            _dataService.SelectedPoint = closestPoint;
        }
        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point canvasPos = e.GetPosition(ImageCanvas);

            Point3D? pointToRemove = null;
            double minDistance = double.MaxValue;
            const double clickRadius = 10;

            foreach (var point in _dataService.Points)
            {
                double distance = Math.Sqrt(
                    Math.Pow(canvasPos.X - point.X, 2) +
                    Math.Pow(canvasPos.Y - point.Y, 2));

                if (distance < clickRadius && distance < minDistance)
                {
                    minDistance = distance;
                    pointToRemove = point;
                }
            }

            if (pointToRemove != null)
            {
                _dataService.Points.Remove(pointToRemove);
                Triangulate_Click(sender, e); 
            }
        }
        public void Triangulate_Click(object sender, RoutedEventArgs e)
        {
            var triangles = !isFromObj 
                ? new ObservableCollection<MeshTriangle>(TriangulationService.Triangulate(_dataService.Points))
                : _dataService.Triangles;

            if (!isFromObj)
            {
                _dataService.Triangles.Clear();
                foreach (var triangle in triangles)
                {
                    _dataService.Triangles.Add(triangle);
                }
            }


            foreach (Window window in Application.Current.Windows)
            {
                if (window is Viewer3DWindow viewer)
                {
                    viewer.Refresh();
                }
            }
        }
        private void ExportObj_Click(object sender, RoutedEventArgs e)
        {
            //var triangles = TriangulationService.Triangulate(_dataService.Points);
            var triangles = _dataService.Triangles;
            isFromObj = true;

            var vertices = new List<Vector3>();
            var vertexIndices = new Dictionary<Vector3, int>();
            var sb = new StringBuilder();
            int index = 1;

            foreach (var triangle in triangles)
            {
                var points = new[] { triangle.A, triangle.B, triangle.C };

                foreach (var p in points)
                {
                    var v = new Vector3((float)p.X / 100.0f, (float)p.Y / 100.0f, (float)p.Z / 100.0f);
                    if (!vertexIndices.ContainsKey(v))
                    {
                        vertexIndices[v] = index++;
                        sb.AppendLine($"v {v.X.ToString(CultureInfo.InvariantCulture)} {v.Y.ToString(CultureInfo.InvariantCulture)} {v.Z.ToString(CultureInfo.InvariantCulture)}");
                    }
                }
            }

            foreach (var triangle in triangles)
            {
                var a = new Vector3((float)triangle.A.X / 100.0f, (float)triangle.A.Y / 100.0f, (float)triangle.A.Z / 100.0f);
                var b = new Vector3((float)triangle.B.X / 100.0f, (float)triangle.B.Y / 100.0f, (float)triangle.B.Z / 100.0f);
                var c = new Vector3((float)triangle.C.X / 100.0f, (float)triangle.C.Y / 100.0f, (float)triangle.C.Z / 100.0f);

                sb.AppendLine($"f {vertexIndices[a]} {vertexIndices[b]} {vertexIndices[c]}");
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "OBJ Files|*.obj",
                FileName = "model.obj"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, sb.ToString());
                MessageBox.Show("Экспорт завершён", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ImportObj_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "OBJ files (*.obj)|*.obj"
            };

            if (dialog.ShowDialog() != true)
                return;

            const double scaleFactor = 100.0;
            var points = new List<Point3D>();
            var triangles = new List<MeshTriangle>();
            isFromObj = true;

            var lines = File.ReadLines(dialog.FileName);

            foreach (var line in lines)
            {
                if (line.StartsWith("v ", StringComparison.Ordinal))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4 &&
                        double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                        double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var y) &&
                        double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                    {
                        points.Add(new Point3D
                        {
                            X = x * scaleFactor,
                            Y = y * scaleFactor,
                            Z = z * scaleFactor
                        });
                    }
                }
            }

            foreach (var line in lines)
            {
                if (!line.StartsWith("f ", StringComparison.Ordinal))
                    continue;

                var parts = line.Substring(2).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var indices = new List<int>(parts.Length);

                foreach (var part in parts)
                {
                    var vertexPart = part.Split('/')[0];
                    if (int.TryParse(vertexPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var idx))
                    {
                        idx = (idx < 0) ? points.Count + idx : idx - 1;

                        if (idx >= 0 && idx < points.Count)
                            indices.Add(idx);
                    }
                }

                for (int i = 1; i + 1 < indices.Count; i++)
                {
                    triangles.Add(new MeshTriangle
                    {
                        A = points[indices[0]],
                        B = points[indices[i]],
                        C = points[indices[i + 1]]
                    });
                }
            }

            _dataService.Triangles.Clear();
            _dataService.Points.Clear();

            foreach (var triangle in triangles)
            {
                _dataService.Triangles.Add(triangle);
                _dataService.Points.Add(triangle.A);
                _dataService.Points.Add(triangle.B);
                _dataService.Points.Add(triangle.C);
            }

            /*
            StringBuilder sb = new();
            foreach (var triangle in _dataService.Triangles)
            {
                sb.AppendLine($"Треугольник :");
                sb.AppendLine($"  A: ({triangle.A.X:F2}, {triangle.A.Y:F2}, {triangle.A.Z:F2})");
                sb.AppendLine($"  B: ({triangle.B.X:F2}, {triangle.B.Y:F2}, {triangle.B.Z:F2})");
                sb.AppendLine($"  C: ({triangle.C.X:F2}, {triangle.C.Y:F2}, {triangle.C.Z:F2})");
                sb.AppendLine();
            }
            MessageBox.Show(sb.ToString());
            */

            Refresh();
            Triangulate_Click(sender, e);
        }

        private void ImageCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoom = e.Delta > 0 ? 1.1 : 1 / 1.1;
            var position = e.GetPosition(ImageCanvas);

            ImageScale.ScaleX *= zoom;
            ImageScale.ScaleY *= zoom;

            ImageTranslate.X = (ImageTranslate.X - position.X) * zoom + position.X;
            ImageTranslate.Y = (ImageTranslate.Y - position.Y) * zoom + position.Y;

            e.Handled = true;
        }

        private void ImageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Space)) 
            {
                _lastMousePos = e.GetPosition(this);
                _isDragging = true;
                Mouse.Capture(ImageCanvas);
            }
            else
            {
                Canvas_MouseLeftButtonDown_Base(sender, e);
            }
        }

        private void Canvas_MouseLeftButtonDown_Base(object sender, MouseButtonEventArgs e)
        {
            Point canvasPos = e.GetPosition(ImageCanvas);
            GeneralTransform imageTransform = MainImage.TransformToAncestor(ImageCanvas);
            Point imageTopLeft = imageTransform.Transform(new Point(0, 0));

            double renderedWidth = MainImage.ActualWidth;
            double renderedHeight = MainImage.ActualHeight;

            if (canvasPos.X < imageTopLeft.X || canvasPos.X > imageTopLeft.X + renderedWidth ||
                canvasPos.Y < imageTopLeft.Y || canvasPos.Y > imageTopLeft.Y + renderedHeight)
                return;

            var newPoint = new Point3D { X = canvasPos.X, Y = canvasPos.Y, Z = 0 };
            _dataService.Points.Add(newPoint);
            _dataService.SelectedPoint = newPoint;
            Triangulate_Click(sender, e);
        }

        private void ImageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPos = e.GetPosition(this);
                System.Windows.Vector delta = currentPos - _lastMousePos;

                ImageTranslate.X += delta.X;
                ImageTranslate.Y += delta.Y;

                _lastMousePos = currentPos;
            }
        }

        private void ImageCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);
            }
        }
        public void Refresh()
        {
            PointsControl.Items.Refresh();
        }

    }
}
