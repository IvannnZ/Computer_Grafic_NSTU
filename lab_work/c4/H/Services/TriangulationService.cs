using DelaunatorSharp;
using Photo3DEditor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photo3DEditor.Services
{
    public static class TriangulationService
    {
        public static List<MeshTriangle> Triangulate(IEnumerable<Point3D> points)
        {
            var pointList = points.ToList(); 
            if (pointList.Count < 3) return new List<MeshTriangle>();

            IPoint[] delPoints = pointList
                .Select(p => (IPoint)new Point(p.X, p.Y))
                .ToArray();

            Delaunator delaunator = new Delaunator(delPoints);

            List<MeshTriangle> triangles = new();
            for (int i = 0; i < delaunator.Triangles.Length; i += 3)
            {
                triangles.Add(new MeshTriangle
                {
                    A = pointList[delaunator.Triangles[i]],
                    B = pointList[delaunator.Triangles[i + 1]],
                    C = pointList[delaunator.Triangles[i + 2]]
                });
            }

            return triangles;
        }

        public static List<MeshTriangle> TriangulateWithInfo(IEnumerable<Point3D> points, out string triangleInfo)
        {
            var pointList = points.ToList();
            List<MeshTriangle> triangles = new();
            StringBuilder sb = new();

            triangleInfo = "";

            if (pointList.Count < 3)
                return triangles;

            // === Фильтрация по окну (подставь свои реальные размеры!) ===
            double minX = 0, maxX = 1000;
            double minY = 0, maxY = 800;

            var filteredPoints = pointList
                .Where(p => p.X >= minX && p.X <= maxX &&
                            p.Y >= minY && p.Y <= maxY)
                .ToList();

            if (filteredPoints.Count < 3)
            {
                triangleInfo = "Недостаточно точек после фильтрации для триангуляции.";
                return triangles;
            }

            // === Подготовка к Delaunay ===
            IPoint[] delPoints = filteredPoints
                .Select(p => (IPoint)new Point(p.X, p.Y))
                .ToArray();

            var delaunator = new Delaunator(delPoints);

            for (int i = 0; i < delaunator.Triangles.Length; i += 3)
            {
                var triangle = new MeshTriangle
                {
                    A = filteredPoints[delaunator.Triangles[i]],
                    B = filteredPoints[delaunator.Triangles[i + 1]],
                    C = filteredPoints[delaunator.Triangles[i + 2]]
                };
                triangles.Add(triangle);

                sb.AppendLine($"Треугольник {triangles.Count}:");
                sb.AppendLine($"  A: ({triangle.A.X:F2}, {triangle.A.Y:F2}, {triangle.A.Z:F2})");
                sb.AppendLine($"  B: ({triangle.B.X:F2}, {triangle.B.Y:F2}, {triangle.B.Z:F2})");
                sb.AppendLine($"  C: ({triangle.C.X:F2}, {triangle.C.Y:F2}, {triangle.C.Z:F2})");
                sb.AppendLine();
            }

            triangleInfo = sb.ToString();
            return triangles;
        }


    }
}