using System.Collections.ObjectModel;
using System.ComponentModel;
using Photo3DEditor.Models;

namespace Photo3DEditor.Services
{
    public class DataService : INotifyPropertyChanged
    {
        private Point3D? _selectedPoint;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Point3D? SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                if (_selectedPoint != value)
                {
                    _selectedPoint = value;
                    OnPropertyChanged(nameof(SelectedPoint));
                    OnPropertyChanged(nameof(SelectedPointAsEnumerable)); 
                }
            }
        }

        public IEnumerable<Point3D> SelectedPointAsEnumerable =>
            SelectedPoint != null ? new[] { SelectedPoint } : Enumerable.Empty<Point3D>();

        public ObservableCollection<Point3D> Points { get; } = new();

        public ObservableCollection<MeshTriangle> Triangles { get; } = new();

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
