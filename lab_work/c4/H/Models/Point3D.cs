using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Photo3DEditor.Models
{
    public class Point3D : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _z;
        private int _index;
        private bool _isActive;

        public event PropertyChangedEventHandler? PropertyChanged;

        

        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                    OnPropertyChanged(nameof(DisplayInfo));
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                    OnPropertyChanged(nameof(DisplayInfo));
                }
            }
        }

        public double Z
        {
            get => _z;
            set
            {
                if (_z != value)
                {
                    _z = value;
                    OnPropertyChanged(nameof(Z));
                    OnPropertyChanged(nameof(DisplayInfo));
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                    OnPropertyChanged(nameof(DisplayInfo));
                }
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index));
                    OnPropertyChanged(nameof(DisplayInfo));
                }
            }
        }

        public string DisplayInfo =>
            $"Точка {Index + 1}\nX: {X:0.##}, Y: {Y:0.##}, Z: {Z:0.##}";

        public System.Windows.Point Position => new System.Windows.Point(_x, _y);

        public int V { get; }

        public Vector3D ToVector() => new Vector3D(X, Y, Z);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
