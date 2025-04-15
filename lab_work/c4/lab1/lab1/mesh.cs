using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Custom3DEngine
{
    public class Triangle3D
    {
        public Vector4D[] Vertices { get; } = new Vector4D[3];
        public Vector4D Normal { get; set; }

        public Triangle3D(Vector4D a, Vector4D b, Vector4D c)
        {
            Vertices[0] = a;
            Vertices[1] = b;
            Vertices[2] = c;
        }

        public List<Triangle3D> ClipAgainstPlane(Vector4D planePoint, Vector4D planeNormal)
        {
            planeNormal.Normalize();
            
            Func<Vector4D, float> distance = p => 
                Vector4D.Dot(planeNormal, p) - Vector4D.Dot(planeNormal, planePoint);

            var inside = new List<Vector4D>(3);
            var outside = new List<Vector4D>(3);

            foreach (var v in Vertices)
            {
                if (distance(v) >= 0) inside.Add(v);
                else outside.Add(v);
            }

            if (inside.Count == 0) return new List<Triangle3D>();
            if (inside.Count == 3) return new List<Triangle3D> { this };

            if (inside.Count == 1)
            {
                Vector4D i1 = Intersect(planePoint, planeNormal, inside[0], outside[0]);
                Vector4D i2 = Intersect(planePoint, planeNormal, inside[0], outside[1]);
                return new List<Triangle3D> { new Triangle3D(inside[0], i1, i2) };
            }

            Vector4D i3 = Intersect(planePoint, planeNormal, inside[0], outside[0]);
            Vector4D i4 = Intersect(planePoint, planeNormal, inside[1], outside[0]);
            
            return new List<Triangle3D> 
            {
                new Triangle3D(inside[0], inside[1], i3),
                new Triangle3D(inside[1], i3, i4)
            };
        }

        private Vector4D Intersect(Vector4D planePoint, Vector4D planeNormal, 
                                 Vector4D start, Vector4D end)
        {
            planeNormal.Normalize();
            float pd = -Vector4D.Dot(planeNormal, planePoint);
            float ad = Vector4D.Dot(start, planeNormal);
            float bd = Vector4D.Dot(end, planeNormal);
            float t = (-pd - ad) / (bd - ad);
            return start + (end - start) * t;
        }
    }

    public class Mesh3D
    {
        public List<Vector4D> Vertices { get; } = new List<Vector4D>();
        public List<Triangle3D> Triangles { get; } = new List<Triangle3D>();

        public bool LoadFromFile(string path)
        {
            try
            {
                foreach (string line in File.ReadLines(path))
                {
                    string[] parts = line.Trim().Split();
                    if (parts.Length == 0) continue;

                    switch (parts[0])
                    {
                        case "v":
                            if (parts.Length >= 4)
                                Vertices.Add(new Vector4D(
                                    float.Parse(parts[1]),
                                    float.Parse(parts[2]),
                                    float.Parse(parts[3])));
                            break;
                        case "f":
                            if (parts.Length >= 4)
                                Triangles.Add(new Triangle3D(
                                    Vertices[int.Parse(parts[1]) - 1],
                                    Vertices[int.Parse(parts[2]) - 1],
                                    Vertices[int.Parse(parts[3]) - 1]));
                            break;
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public void CreateCube(float size = 1f)
        {
            Vertices.Clear();
            Triangles.Clear();

            float s = size / 2;
            Vertices.AddRange(new[]
            {
                new Vector4D(-s, -s, -s), new Vector4D(-s, s, -s),
                new Vector4D(s, s, -s), new Vector4D(s, -s, -s),
                new Vector4D(-s, -s, s), new Vector4D(-s, s, s),
                new Vector4D(s, s, s), new Vector4D(s, -s, s)
            });

            // Добавление треугольников для куба
            // (аналогично оригиналу, но с новыми индексами)
        }
    }
}