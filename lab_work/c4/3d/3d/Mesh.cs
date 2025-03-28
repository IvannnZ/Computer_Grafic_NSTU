using System;
using System.Collections.Generic;
using System.IO;

public struct Triangle
{
    public Vec4[] Points;
    public Vec4 Normal;

    // Конструкторы
    public Triangle(Vec4 p1, Vec4 p2, Vec4 p3)
    {
        Points = new Vec4[3] { p1, p2, p3 };
        Normal = Vec4.Cross(p2 - p1, p3 - p1);
            Normal.Normalize();
    }

    public Triangle(Vec4[] points)
    {
        if (points.Length != 3)
            throw new ArgumentException("Triangle must have exactly 3 points");
        
        Points = new Vec4[3];
        Array.Copy(points, Points, 3);
        Normal = Vec4.Cross(Points[1] - Points[0], Points[2] - Points[0]);
            Normal.Normalize();
    }

    // Метод для клиппинга треугольника плоскостью
    public List<Triangle> ClipAgainstPlane(Vec4 planePoint, Vec4 planeNormal)
    {
        planeNormal.Normalize();
        
        // Функция для вычисления расстояния до плоскости
        float Distance(Vec4 point)
        {
            return Vec4.Dot(planeNormal, point) - Vec4.Dot(planeNormal, planePoint);
        }

        Vec4[] insidePoints = new Vec4[3];
        int insideCount = 0;
        Vec4[] outsidePoints = new Vec4[3]; 
        int outsideCount = 0;

        // Классификация точек
        for (int i = 0; i < 3; i++)
        {
            float dist = Distance(Points[i]);
            if (dist >= 0)
                insidePoints[insideCount++] = Points[i];
            else
                outsidePoints[outsideCount++] = Points[i];
        }

        // Все точки снаружи - отбрасываем треугольник
        if (insideCount == 0) 
            return new List<Triangle>();

        // Все точки внутри - оставляем как есть
        if (insideCount == 3) 
            return new List<Triangle> { this };

        // Одна точка внутри - создаем 1 новый треугольник
        if (insideCount == 1 && outsideCount == 2)
        {
            Triangle result = new Triangle(
                insidePoints[0],
                Vec4.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0], out _),
                Vec4.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[1], out _)
            );
            result.Normal = Normal;
            return new List<Triangle> { result };
        }

        // Две точки внутри - создаем 2 новых треугольника
        if (insideCount == 2 && outsideCount == 1)
        {
            Vec4 intersection = Vec4.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0], out _);
            
            Triangle result1 = new Triangle(
                insidePoints[0],
                insidePoints[1],
                intersection
            );
            result1.Normal = Normal;
            
            Triangle result2 = new Triangle(
                insidePoints[1],
                intersection,
                Vec4.IntersectPlane(planePoint, planeNormal, insidePoints[1], outsidePoints[0], out _)
            );
            result2.Normal = Normal;
            
            return new List<Triangle> { result1, result2 };
        }

        throw new InvalidOperationException("Unexpected case in triangle clipping");
    }
}


public class Mesh
{
    public List<Triangle> Tris { get; private set; } = new List<Triangle>();

    // Конструкторы
    public Mesh() { }

    public Mesh(List<Triangle> triangles)
    {
        Tris = new List<Triangle>(triangles);
    }

    // Загрузка из OBJ файла
    public bool LoadFromFile(string filename)
    {
        Tris = new List<Triangle>();
        List<Vec4> vertices = new List<Vec4>();

        try
        {
            string[] lines = File.ReadAllLines(filename);
            
            foreach (string line in lines)
            {
                if (line.StartsWith("v "))
                {
                    // Обработка вершин
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);
                        float z = float.Parse(parts[3]);
                        vertices.Add(new Vec4(x, y, z));
                    }
                }
                else if (line.StartsWith("f "))
                {
                    // Обработка граней
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        int[] indices = new int[3];
                        
                        // Обработка форматов: f v1 v2 v3 или f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
                        for (int i = 0; i < 3; i++)
                        {
                            string facePart = parts[i + 1];
                            string vertexIndex = facePart.Split('/')[0];
                            indices[i] = int.Parse(vertexIndex) - 1; // OBJ использует 1-based индексы
                        }
                        
                        if (indices[0] < vertices.Count && indices[1] < vertices.Count && indices[2] < vertices.Count)
                        {
                            Tris.Add(new Triangle(
                                vertices[indices[0]],
                                vertices[indices[1]],
                                vertices[indices[2]]));
                        }
                    }
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

    // Создание куба
    public void DefineAsCube()
    {
        Tris = new List<Triangle>
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


//
// public class Mesh
// {
//     public List<Triangle> Tris { get; private set; }
//
//     public Mesh()
//     {
//         Tris = new List<Triangle>();
//     }
//
//     public Mesh(List<Triangle> triangles)
//     {
//         Tris = new List<Triangle>(triangles);
//     }
//
//     public bool LoadFromFile(string fileName)
//     {
//         Tris = new List<Triangle>();
//         List<Vec4> verts = new List<Vec4>();
//
//         try
//         {
//             using (StreamReader file = new StreamReader(fileName))
//             {
//                 string line;
//                 while ((line = file.ReadLine()) != null)
//                 {
//                     if (line.StartsWith("v "))
//                     {
//                         string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
//                         if (parts.Length >= 4)
//                         {
//                             Vec4 v = new Vec4(
//                                 float.Parse(parts[1]),
//                                 float.Parse(parts[2]),
//                                 float.Parse(parts[3]));
//                             verts.Add(v);
//                         }
//                     }
//                     else if (line.StartsWith("f "))
//                     {
//                         string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
//                         if (parts.Length >= 4)
//                         {
//                             int[] f = new int[3];
//                             
//                             // Обработка формата f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
//                             if (parts[1].Contains("/"))
//                             {
//                                 for (int i = 0; i < 3; i++)
//                                 {
//                                     string[] vertexParts = parts[i + 1].Split('/');
//                                     f[i] = int.Parse(vertexParts[0]);
//                                 }
//                             }
//                             else
//                             {
//                                 for (int i = 0; i < 3; i++)
//                                 {
//                                     f[i] = int.Parse(parts[i + 1]);
//                                 }
//                             }
//
//                             Triangle t = new Triangle(
//                                 verts[f[0] - 1],
//                                 verts[f[1] - 1],
//                                 verts[f[2] - 1]);
//                             Tris.Add(t);
//                         }
//                     }
//                 }
//             }
//             return true;
//         }
//         catch
//         {
//             return false;
//         }
//     }
//
//     public void DefineAsCube()
//     {
//         Tris = new List<Triangle>
//         {
//             // SOUTH
//             new Triangle(new Vec4(0.0f, 0.0f, 0.0f), new Vec4(0.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f)),
//             new Triangle(new Vec4(0.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f), new Vec4(1.0f, 0.0f, 0.0f)),
//             
//             // EAST
//             new Triangle(new Vec4(1.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f)),
//             new Triangle(new Vec4(1.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, 0.0f, 1.0f)),
//             
//             // NORTH
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f)),
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(0.0f, 0.0f, 1.0f)),
//             
//             // WEST
//             new Triangle(new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(0.0f, 1.0f, 0.0f)),
//             new Triangle(new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 0.0f), new Vec4(0.0f, 0.0f, 0.0f)),
//             
//             // TOP
//             new Triangle(new Vec4(0.0f, 1.0f, 0.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f)),
//             new Triangle(new Vec4(0.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, 0.0f)),
//             
//             // BOTTOM
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 0.0f)),
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 0.0f), new Vec4(1.0f, 0.0f, 0.0f))
//         };
//     }
// }



// using System;
// using System.Collections.Generic;
// using System.Numerics;
// using System.IO;
//
//
//
// public struct Triangle
// {
//     public Vec4[] Points { get; } = new Vec4[3];
//     // public Vec4[] Points; // 3 точки треугольника
//     public Vec4 Normal;
//     
//     // Конструкторы
//     
//     public Triangle(Vec4 p1, Vec4 p2, Vec4 p3)
//     {
//         Points = new Vec4[3] { p1, p2, p3 };
//         Normal = Vec4.Zero;
//     }
//     public Triangle(Triangle source)
//     {
//         Points = new Vec4[3];
//         Array.Copy(source.Points, Points, 3);
//         Normal = source.Normal;
//     }
//     
//     public Triangle(Vec4[] points)
//     {
//         if (points.Length != 3)
//             throw new ArgumentException("Triangle must have exactly 3 points");
//             
//         Points = new Vec4[3];
//         Array.Copy(points, Points, 3);
//         Normal = Vec4.Zero;
//     }
//     
//     // Метод для клиппинга треугольника плоскостью
//     public List<Triangle> ClipAgainstPlane(Vec4 planePoint, Vec4 planeNormal)
//     {
//         planeNormal = Vec4.Normalize(planeNormal);
//         
//         // Функция для вычисления расстояния от точки до плоскости
//         float Distance(Vec4 point)
//         {
//             return Vec4.Dot(planeNormal, point) - Vec4.Dot(planeNormal, planePoint);
//         }
//         
//         Vec4[] insidePoints = new Vec4[3];
//         int insideCount = 0;
//         Vec4[] outsidePoints = new Vec4[3];
//         int outsideCount = 0;
//         
//         // Классифицируем точки
//         for (int i = 0; i < 3; i++)
//         {
//             float dist = Distance(Points[i]);
//             if (dist >= 0)
//                 insidePoints[insideCount++] = Points[i];
//             else
//                 outsidePoints[outsideCount++] = Points[i];
//         }
//         
//         // Различные случаи
//         if (insideCount == 0)
//             return new List<Triangle>(); // Треугольник полностью снаружи
//         
//         if (insideCount == 3)
//             return new List<Triangle> { this }; // Треугольник полностью внутри
//         
//         if (insideCount == 1 && outsideCount == 2)
//         {
//             // Одна точка внутри - создаем 1 новый треугольник
//             Triangle result = new Triangle(
//                 insidePoints[0],
//                 IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]),
//                 IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[1])
//             );
//             result.Normal = this.Normal;
//             return new List<Triangle> { result };
//         }
//         
//         if (insideCount == 2 && outsideCount == 1)
//         {
//             // Две точки внутри - создаем 2 новых треугольника
//             Vec4 intersection = IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]);
//             
//             Triangle result1 = new Triangle(
//                 insidePoints[0],
//                 insidePoints[1],
//                 intersection
//             );
//             result1.Normal = this.Normal;
//             
//             Triangle result2 = new Triangle(
//                 insidePoints[1],
//                 intersection,
//                 IntersectPlane(planePoint, planeNormal, insidePoints[1], outsidePoints[0])
//             );
//             result2.Normal = this.Normal;
//             
//             return new List<Triangle> { result1, result2 };
//         }
//         
//         throw new InvalidOperationException("Unexpected case in triangle clipping");
//     }
//     
//     // Метод для нахождения пересечения линии с плоскостью
//     private static Vec4 IntersectPlane(Vec4 planePoint, Vec4 planeNormal, 
//                                         Vec4 lineStart, Vec4 lineEnd)
//     {
//         planeNormal = Vec4.Normalize(planeNormal);
//         Vec4 lineDir = Vec4.Normalize(lineEnd - lineStart);
//         
//         float planeDot = Vec4.Dot(planeNormal, planePoint);
//         float startDot = Vec4.Dot(lineStart, planeNormal);
//         float endDot = Vec4.Dot(lineEnd, planeNormal);
//         
//         float t = (planeDot - startDot) / (endDot - startDot);
//         return lineStart + t * (lineEnd - lineStart);
//     }
// }
//
//
// public class Mesh
// {
//     public List<Triangle> Tris { get; private set; }
//
//     public Mesh()
//     {
//         Tris = new List<Triangle>();
//     }
//
//     public Mesh(List<Triangle> triangles)
//     {
//         Tris = new List<Triangle>(triangles);
//     }
//
//     public bool LoadFromFile(string fileName)
//     {
//         Tris = new List<Triangle>();
//         List<Vec4> verts = new List<Vec4>();
//
//         try
//         {
//             using (StreamReader file = new StreamReader(fileName))
//             {
//                 string line;
//                 while ((line = file.ReadLine()) != null)
//                 {
//                     if (line.StartsWith("v "))
//                     {
//                         string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
//                         if (parts.Length >= 4)
//                         {
//                             Vec4 v = new Vec4(
//                                 float.Parse(parts[1]),
//                                 float.Parse(parts[2]),
//                                 float.Parse(parts[3]));
//                             verts.Add(v);
//                         }
//                     }
//                     else if (line.StartsWith("f "))
//                     {
//                         string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
//                         if (parts.Length >= 4)
//                         {
//                             int[] f = new int[3];
//                             
//                             // Обработка формата f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
//                             if (parts[1].Contains("/"))
//                             {
//                                 for (int i = 0; i < 3; i++)
//                                 {
//                                     string[] vertexParts = parts[i + 1].Split('/');
//                                     f[i] = int.Parse(vertexParts[0]);
//                                 }
//                             }
//                             else
//                             {
//                                 for (int i = 0; i < 3; i++)
//                                 {
//                                     f[i] = int.Parse(parts[i + 1]);
//                                 }
//                             }
//
//                             Triangle t = new Triangle(
//                                 verts[f[0] - 1],
//                                 verts[f[1] - 1],
//                                 verts[f[2] - 1]);
//                             Tris.Add(t);
//                         }
//                     }
//                 }
//             }
//             return true;
//         }
//         catch
//         {
//             return false;
//         }
//     }
//
//     public void DefineAsCube()
//     {
//         Tris = new List<Triangle>
//         {
//             // SOUTH
//             new Triangle(new Vec4(0.0f, 0.0f, 0.0f), new Vec4(0.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f)),
//             new Triangle(new Vec4(0.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f), new Vec4(1.0f, 0.0f, 0.0f)),
//             
//             // EAST
//             new Triangle(new Vec4(1.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f)),
//             new Triangle(new Vec4(1.0f, 0.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, 0.0f, 1.0f)),
//             
//             // NORTH
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f)),
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(0.0f, 0.0f, 1.0f)),
//             
//             // WEST
//             new Triangle(new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(0.0f, 1.0f, 0.0f)),
//             new Triangle(new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 1.0f, 0.0f), new Vec4(0.0f, 0.0f, 0.0f)),
//             
//             // TOP
//             new Triangle(new Vec4(0.0f, 1.0f, 0.0f), new Vec4(0.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, 1.0f)),
//             new Triangle(new Vec4(0.0f, 1.0f, 0.0f), new Vec4(1.0f, 1.0f, 1.0f), new Vec4(1.0f, 1.0f, 0.0f)),
//             
//             // BOTTOM
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 0.0f)),
//             new Triangle(new Vec4(1.0f, 0.0f, 1.0f), new Vec4(0.0f, 0.0f, 0.0f), new Vec4(1.0f, 0.0f, 0.0f))
//         };
//     }
// }