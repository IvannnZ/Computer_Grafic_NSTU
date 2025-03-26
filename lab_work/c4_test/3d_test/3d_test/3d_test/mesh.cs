using System;
using System.Collections.Generic;
using System.IO;

public class Triangle
{
    public Vec3[] P = new Vec3[3];
    public Vec3 Normal;

    public Triangle()
    {
        P[0] = new Vec3();
        P[1] = new Vec3();
        P[2] = new Vec3();
        Normal = new Vec3();
    }
}

public class Mesh
{
    public List<Triangle> Tris = new List<Triangle>();

    public bool LoadFromFile(string filename)
    {
        List<Vec3> vertices = new List<Vec3>();
        try
        {
            foreach (var line in File.ReadAllLines(filename))
            {
                string[] parts = line.Split(' ');
                if (parts.Length == 0) continue;

                if (parts[0] == "v")
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    vertices.Add(new Vec3(x, y, z));
                }
                else if (parts[0] == "f")
                {
                    int v1 = int.Parse(parts[1]) - 1;
                    int v2 = int.Parse(parts[2]) - 1;
                    int v3 = int.Parse(parts[3]) - 1;
                    Tris.Add(new Triangle { P = new Vec3[] { vertices[v1], vertices[v2], vertices[v3] } });
                }
            }
            return true;
        }
        catch
        {
            Console.WriteLine("Failed to load mesh from file.");
            return false;
        }
    }
}