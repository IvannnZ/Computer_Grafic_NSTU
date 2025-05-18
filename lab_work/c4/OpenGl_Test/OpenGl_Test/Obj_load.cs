using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

public class ObjModel
{
    public float[]? Vertices;  // x,y,z
    public float[]? Normals;   // nx,ny,nz
    public int[]? Indices;     // индексы для DrawElements

    public static ObjModel LoadFromFile(string path)
    {
        var verts = new List<(float, float, float)>();
        var norms = new List<(float, float, float)>();
        // var faces = new List<(int, int, int)>();
        var faces = new List<((int v, int n) v1, (int v, int n) v2, (int v, int n) v3)>();

        var lines = File.ReadAllLines(path);

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (line.StartsWith("v "))
            {
                // строка вершины: v x y z
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                verts.Add((x, y, z));
            }
            else if (line.StartsWith("vn "))
            {
                // нормаль вершины: vn nx ny nz
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                float nx = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float ny = float.Parse(parts[2], CultureInfo.InvariantCulture);
                float nz = float.Parse(parts[3], CultureInfo.InvariantCulture);
                norms.Add((nx, ny, nz));
            }
            else if (line.StartsWith("f "))
            {
                var parts = line.Substring(2).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3) throw new Exception("Только треугольники поддерживаются");
                (int v, int n)[] vertexNormal = new (int, int)[3];
                for (int i = 0; i < 3; i++)
                {
                    var comps = parts[i].Split('/');
                    int vertexIndex = int.Parse(comps[0]) - 1;
                    int normalIndex = comps.Length >= 3 && comps[2] != "" ? int.Parse(comps[2]) - 1 : -1;
                    vertexNormal[i] = (vertexIndex, normalIndex);
                }
                faces.Add((vertexNormal[0], vertexNormal[1], vertexNormal[2]));
            }

        }

        // Сборка финальных массивов (простое дублирование вершин)
        var finalVerts = new List<float>();
        var finalNorms = new List<float>();
        var finalIdxs = new List<int>();
        foreach (var (v1, v2, v3) in faces)
        {
            var vertsAndNorms = new[] { v1, v2, v3 };
            foreach (var (vIdx, nIdx) in vertsAndNorms)
            {
                finalVerts.AddRange(new float[] {
                    verts[vIdx].Item1, verts[vIdx].Item2, verts[vIdx].Item3
                });

                if (nIdx >= 0)
                {
                    finalNorms.AddRange(new float[] {
                        norms[nIdx].Item1, norms[nIdx].Item2, norms[nIdx].Item3
                    });
                }
            }

            int baseIndex = finalIdxs.Count > 0 ? finalIdxs.Count : 0;
            finalIdxs.AddRange(new int[] { baseIndex, baseIndex + 1, baseIndex + 2 });
        }


        return new ObjModel {
            Vertices = finalVerts.ToArray(),
            Normals = finalNorms.Count > 0 ? finalNorms.ToArray() : null,
            Indices = finalIdxs.ToArray()
        };
    }
}
