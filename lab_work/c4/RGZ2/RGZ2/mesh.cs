using System;
using System.Collections.Generic;
using System.Numerics;

public class Triangle
{
    public Vec4[] Points { get; private set; } = new Vec4[3];
    public Vec4 Normal { get; set; }

    // Конструкторы
    public Triangle()
    {
    }

    public Triangle(Triangle t)
    {
        this.Normal = t.Normal;
        this.Points = t.Points;
    }

    public Triangle(Vec4[] points)
    {
        if (points.Length != 3)
            throw new ArgumentException("Triangle must have exactly 3 points");
        Points = points;
    }

    public Triangle(Vec4 p1, Vec4 p2, Vec4 p3)
    {
        Points[0] = p1;
        Points[1] = p2;
        Points[2] = p3;
    }

    // Новый конструктор, принимающий Vec4 (использует только x,y,z)
    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Points[0] = new Vec4(p1.X, p1.Y, p1.Z);
        Points[1] = new Vec4(p2.X, p2.Y, p2.Z);
        Points[2] = new Vec4(p3.X, p3.Y, p3.Z);
    }

    public Triangle(OpenTK.Mathematics.Vector3 p1, OpenTK.Mathematics.Vector3 p2, OpenTK.Mathematics.Vector3 p3)
    {
        Points[0] = new Vec4(p1.X, p1.Y, p1.Z);
        Points[1] = new Vec4(p2.X, p2.Y, p2.Z);
        Points[2] = new Vec4(p3.X, p3.Y, p3.Z);}


    /*
     Что делает:
       Отсекает треугольник плоскостью, возвращая:

           Пустой список, если треугольник полностью невидим (лежит по "обратную" сторону плоскости).

           Исходный треугольник, если он полностью видим.

           1 или 2 новых треугольника, если плоскость пересекает исходный.

       Алгоритм:

           Нормализует вектор нормали плоскости.

           Классифицирует вершины треугольника на:

               insidePoints — вершины перед плоскостью (видимые).

               outsidePoints — вершины за плоскостью (невидимые).

           Обрабатывает 4 случая:

               Все вершины невидимы → возвращает пустой список.

               Все вершины видимы → возвращает исходный треугольник.

               1 видимая + 2 невидимые → создает 1 новый треугольник с двумя новыми точками пересечения.

               2 видимые + 1 невидимая → создает 2 новых треугольника, образующих четырехугольник.
     */
    public List<Triangle> ClipAgainstPlane(Vec4 planePoint, Vec4 planeNormal)
    {
        planeNormal.Normalize();

        float DistanceToPlane(Vec4 point)
        {
            return Vec4.Dot(planeNormal, point) - Vec4.Dot(planeNormal, planePoint);
        }

        List<Vec4> insidePoints = new List<Vec4>(3);
        List<Vec4> outsidePoints = new List<Vec4>(3);

        for (int i = 0; i < 3; i++)
        {
            float distance = DistanceToPlane(Points[i]);
            if (distance >= 0)
                insidePoints.Add(Points[i]);
            else
                outsidePoints.Add(Points[i]);
        }

        if (insidePoints.Count == 0)
            return new List<Triangle>();

        if (insidePoints.Count == 3)
            return new List<Triangle> { this };

        if (insidePoints.Count == 1 && outsidePoints.Count == 2)
        {
            Vec4 newPoint1 = IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]);
            Vec4 newPoint2 = IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[1]);

            return new List<Triangle> { new Triangle(insidePoints[0], newPoint1, newPoint2) { Normal = this.Normal } };
        }

        if (insidePoints.Count == 2 && outsidePoints.Count == 1)
        {
            Vec4 newPoint1 = IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]);
            Vec4 newPoint2 = IntersectPlane(planePoint, planeNormal, insidePoints[1], outsidePoints[0]);

            Triangle Triangle1 = new Triangle(insidePoints[0], insidePoints[1], newPoint1) { Normal = this.Normal };
            Triangle Triangle2 = new Triangle(insidePoints[1], newPoint1, newPoint2) { Normal = this.Normal };

            return new List<Triangle> { Triangle1, Triangle2 };
        }

        return new List<Triangle>();
    }


    /*

     Что делает:
       Находит точку пересечения отрезка (lineStart—lineEnd) с плоскостью.
    Возвращает:
        Точку пересечения как Vector3.

        Если отрезок параллелен плоскости, возвращает точку на прямой (но проверка в ClipAgainstPlane исключает этот случай).

     */
    private Vec4 IntersectPlane(Vec4 planePoint, Vec4 planeNormal, Vec4 lineStart, Vec4 lineEnd)
    {
        planeNormal.Normalize();
        float planeD = -Vec4.Dot(planeNormal, planePoint);
        float ad = Vec4.Dot(lineStart, planeNormal);
        float bd = Vec4.Dot(lineEnd, planeNormal);
        float t = (-planeD - ad) / (bd - ad);

        Vec4 lineStartToEnd = lineEnd - lineStart;
        Vec4 lineToIntersect = lineStartToEnd * t;
        return lineStart + lineToIntersect;
    }

    public override string ToString()
    {
        return $"Triangle: {Points[0]}, {Points[1]}, {Points[2]} (Normal: {Normal})";
    }
}


public class Mesh
{
    public List<Vec4> Vertices { get; set; } = new List<Vec4>(); // вершины
    public List<Tuple<int, int>> Lines { get; } = new List<Tuple<int, int>>(); // линии
    public List<Triangle> Triangles { get; set; } = new List<Triangle>(); // плоскости(полигоны)

    public bool LoadFromFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            Vertices.Clear();
            Lines.Clear();
            Triangles.Clear();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) continue;

                switch (parts[0].ToLower())
                {
                    case "v": // Vertex: v x y z
                        if (parts.Length >= 4) // Проверяем, что есть как минимум 3 координаты
                        {
                            Vertices.Add(new Vec4(
                                float.Parse(parts[1]),
                                float.Parse(parts[2]),
                                float.Parse(parts[3])));
                        }

                        break;

                    case "l": // Line: l index1 index2
                        if (parts.Length >= 3)
                        {
                            int index1 = int.Parse(parts[1]) - 1; // Вычитаем 1 для 0-based индекса
                            int index2 = int.Parse(parts[2]) - 1;
                            if (index1 >= 0 && index1 < Vertices.Count &&
                                index2 >= 0 && index2 < Vertices.Count)
                            {
                                Lines.Add(new Tuple<int, int>(index1, index2));
                            }
                        }

                        break;

                    case "f": // Face (полигон): f index1 index2 index3 (в OBJ обычно 'f', а не 'p')
                    case "p": // На случай, если в вашем формате используется 'p'
                        if (parts.Length >= 4)
                        {
                            int index1 = int.Parse(parts[1]) - 1;
                            int index2 = int.Parse(parts[2]) - 1;
                            int index3 = int.Parse(parts[3]) - 1;
                            if (index1 >= 0 && index1 < Vertices.Count &&
                                index2 >= 0 && index2 < Vertices.Count &&
                                index3 >= 0 && index3 < Vertices.Count)
                            {
                                Triangles.Add(new Triangle(
                                    Vertices[index1],
                                    Vertices[index2],
                                    Vertices[index3]));
                            }
                        }

                        break;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file: {ex.Message}");
            return false;
        }
    }

    // public bool LoadFromFile(string filePath)
    // {
    //     try
    //     {
    //         string[] lines = File.ReadAllLines(filePath);
    //
    //         foreach (string line in lines)
    //         {
    //             string trimmed = line.Trim();
    //             if (string.IsNullOrWhiteSpace(trimmed)) continue;
    //
    //             string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //
    //             if (parts.Length == 0) continue;
    //
    //             switch (parts[0].ToLower())
    //             {
    //                 case "v": // Vertex: v x y z
    //                     if (parts.Length == 4)
    //                     {
    //                         Vertices.Add(new Vec4(
    //                             float.Parse(parts[1]),
    //                             float.Parse(parts[2]),
    //                             float.Parse(parts[3])));
    //                     }
    //
    //                     break;
    //
    //                 case "l": // Line: l index1 index2
    //                     if (parts.Length == 3)
    //                     {
    //                         Lines.Add(new Tuple<int, int>(
    //                             int.Parse(parts[1]),
    //                             int.Parse(parts[2])));
    //                     }
    //
    //                     break;
    //
    //                 case "p": // Polygon: p index1 index2 index3
    //                     if (parts.Length == 4)
    //                     {
    //                         Triangles.Add(new Triangle(
    //                             Vertices[int.Parse(parts[1])],
    //                             Vertices[int.Parse(parts[2])],
    //                             Vertices[int.Parse(parts[3])]));
    //                     }
    //
    //                     break;
    //             }
    //         }
    //
    //         return true;
    //     }
    //     catch
    //     {
    //         return false;
    //     }
    // }

    public void DefineAsCube()
    {
        this.Triangles = new List<Triangle>
        {
            // SOUTH
            new Triangle(new Vec4(-1.0f, -1.0f, -1.0f), new Vec4(-1.0f, 1.0f, -1.0f), new Vec4(1.0f, 1.0f, -1.0f)),
            new Triangle(new Vec4(-1.0f, -1.0f, -1.0f), new Vec4(1.0f, 1.0f, -1.0f), new Vec4(1.0f, -1.0f, -1.0f)),

            // EAST
            new Triangle(new Vec4(1.0f, -1.0f, -1.0f), new Vec4(1.0f, 1.0f, -1.0f), new Vec4(1.0f, 1.0f, 1.0f)),
            new Triangle(new Vec4(1.0f, -1.0f, -1.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, -1.0f, 1.0f)),

            // NORTH
            new Triangle(new Vec4(1.0f, -1.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(-1.0f, 1.0f, 1.0f)),
            new Triangle(new Vec4(1.0f, -1.0f, 1.0f), new Vec4(-1.0f, 1.0f, 1.0f), new Vec4(-1.0f, -1.0f, 1.0f)),

            // WEST
            new Triangle(new Vec4(-1.0f, -1.0f, 1.0f), new Vec4(-1.0f, 1.0f, 1.0f), new Vec4(-1.0f, 1.0f, -1.0f)),
            new Triangle(new Vec4(-1.0f, -1.0f, 1.0f), new Vec4(-1.0f, 1.0f, -1.0f), new Vec4(-1.0f, -1.0f, -1.0f)),

            // TOP
            new Triangle(new Vec4(-1.0f, 1.0f, -1.0f), new Vec4(-1.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f)),
            new Triangle(new Vec4(-1.0f, 1.0f, -1.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, -1.0f)),

            // BOTTOM
            new Triangle(new Vec4(1.0f, -1.0f, 1.0f), new Vec4(-1.0f, -1.0f, 1.0f), new Vec4(-1.0f, -1.0f, -1.0f)),
            new Triangle(new Vec4(1.0f, -1.0f, 1.0f), new Vec4(-1.0f, -1.0f, -1.0f), new Vec4(1.0f, -1.0f, -1.0f))
        };
    }

    public static Mesh Triangulate(Vector2[] points)
    {
        if (points == null || points.Length < 3)
            throw new ArgumentException("Нужно минимум 3 точки для триангуляции.");

        // 1. Найдем выпуклую оболочку (алгоритм Грэхема)
        List<Vector2> hull = ComputeConvexHull(points);

        Mesh mesh = new Mesh();

        // 2. Преобразуем в Vec4 вершины
        foreach (var pt in hull)
            mesh.Vertices.Add(new Vec4(pt.X, pt.Y, 0f));

        // 3. Триангулируем веером: вершина 0 и пары (i, i+1)
        for (int i = 1; i < hull.Count - 1; i++)
        {
            var t = new Triangle(mesh.Vertices[0], mesh.Vertices[i], mesh.Vertices[i + 1]);
            mesh.Triangles.Add(t);
        }

        return mesh;
    }

    private static List<Vector2> ComputeConvexHull(Vector2[] points)
    {
        List<Vector2> sorted = new List<Vector2>(points);
        sorted.Sort((a, b) =>
            a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));

        List<Vector2> lower = new List<Vector2>();
        foreach (var p in sorted)
        {
            while (lower.Count >= 2 &&
                   Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }

        List<Vector2> upper = new List<Vector2>();
        for (int i = sorted.Count - 1; i >= 0; i--)
        {
            Vector2 p = sorted[i];
            while (upper.Count >= 2 &&
                   Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }

        // Удалим дублирующие точки на стыке
        upper.RemoveAt(upper.Count - 1);
        lower.RemoveAt(lower.Count - 1);

        lower.AddRange(upper);
        return lower;
    }

    private static float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
    }

    public float[] GetVertexArray()
    {
        List<float> vertices = new List<float>();
        foreach (var tri in this.Triangles)
        {
            foreach (var v in tri.Points)
            {
                vertices.Add(v.x);
                vertices.Add(v.y);
                vertices.Add(v.z);
            }
        }

        return vertices.ToArray();
    }
    
    public float[] GetVertexArrayWithNormals()
    {
        var list = new List<float>();
        foreach (var tri in Triangles)
        {
            Vec4 edge1 = tri.Points[1] - tri.Points[0];
            Vec4 edge2 = tri.Points[2] - tri.Points[0];
            Vec4 normal = Vec4.Cross(edge1, edge2);
            normal.Normalize();

            foreach (var v in tri.Points)
            {
                list.Add(v.x);
                list.Add(v.y);
                list.Add(v.z);

                list.Add(normal.x);
                list.Add(normal.y);
                list.Add(normal.z);
            }
        }
        return list.ToArray();
    }

}