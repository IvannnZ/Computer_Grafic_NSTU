using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Photo3DEditor.Models
{
    public class MeshTriangle
    {
        public required Point3D A { get; init; }
        public required Point3D B { get; init; }
        public required Point3D C { get; init; }

        public PointCollection PathPoints =>
            new()
            {
                new System.Windows.Point(A.X, A.Y),
                new System.Windows.Point(B.X, B.Y),
                new System.Windows.Point(C.X, C.Y)
            };
    }
}
