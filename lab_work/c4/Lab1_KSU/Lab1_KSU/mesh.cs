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
    public List<Vec4> Vertices { get; } = new List<Vec4>(); // вершины
    public List<Tuple<int, int>> Lines { get; } = new List<Tuple<int, int>>(); // линии
    public List<Triangle> Triangles { get; private set; } = new List<Triangle>(); // плоскости(полигоны)

    public bool LoadFromFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                string[] parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) continue;

                switch (parts[0].ToLower())
                {
                    case "v": // Vertex: v x y z
                        if (parts.Length == 4)
                        {
                            Vertices.Add(new Vec4(
                                float.Parse(parts[1]),
                                float.Parse(parts[2]),
                                float.Parse(parts[3])));
                        }

                        break;

                    case "l": // Line: l index1 index2
                        if (parts.Length == 3)
                        {
                            Lines.Add(new Tuple<int, int>(
                                int.Parse(parts[1]),
                                int.Parse(parts[2])));
                        }

                        break;

                    case "p": // Polygon: p index1 index2 index3
                        if (parts.Length == 4)
                        {
                            Triangles.Add(new Triangle(
                                Vertices[int.Parse(parts[1])],
                                Vertices[int.Parse(parts[2])],
                                Vertices[int.Parse(parts[3])]));
                        }

                        break;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public void DefineAsCube()
    {
        this.Triangles = new List<Triangle>
        {
            // SOUTH
            new Triangle(new Vec4(0.0f, 0.0f, 0.0f), new Vec4(0.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f)),
            new Triangle(new Vec4(0.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f), new Vec4(1.0f, 0.0f, 0.0f)),

            // EAST
            new Triangle(new Vec4(1.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f)),
            new Triangle(new Vec4(1.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, 0.0f, 1.0f)),

            // NORTH
            new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f)),
            new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(0.0f, 0.0f, 1.0f)),

            // WEST
            new Triangle(new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(0.0f, 1.0f, 0.0f)),
            new Triangle(new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 0.0f), new Vec4(0.0f, 0.0f, 0.0f)),

            // TOP
            new Triangle(new Vec4(0.0f, 1.0f, 0.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f)),
            new Triangle(new Vec4(0.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, 0.0f)),

            // BOTTOM
            new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 0.0f)),
            new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 0.0f), new Vec4(1.0f, 0.0f, 0.0f))
        };
    }
}