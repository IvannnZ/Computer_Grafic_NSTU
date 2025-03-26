using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

class Program
{
    static void Main()
    {
        Vec3 cameraLoc = new Vec3(0, -2.0f, -5);
        Mat4x4 projMat = Mat4x4.GetProjMat(1920, 1080, 0.1f, 1000, 90);
        
        Mesh testMesh = new Mesh();
        // if (!testMesh.LoadFromFile("1.obj")) return;
        testMesh.Tris;
        float theta = 0;
        bool allowRotation = true;

        RenderWindow window = new RenderWindow(new VideoMode(1920, 1080), "3D Engine");
        window.SetVerticalSyncEnabled(true);

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(Color.White);

            List<Triangle> toDraw = new List<Triangle>();
            foreach (var tri in testMesh.Tris)
            {
                Triangle transformed = new Triangle();
                for (int i = 0; i < 3; i++)
                {
                    transformed.P[i] = Mat4x4.GetRotY(theta) * tri.P[i];
                    transformed.P[i] = Mat4x4.GetRotZ((float)Math.PI) * transformed.P[i];
                }

                transformed.Normal = Vec3.CrossProd(transformed.P[2] - transformed.P[0], transformed.P[1] - transformed.P[0]);
                transformed.Normal.Norm();
                toDraw.Add(transformed);
            }

            toDraw.Sort((t1, t2) =>
                ((t2.P[0].Z + t2.P[1].Z + t2.P[2].Z) / 3).CompareTo((t1.P[0].Z + t1.P[1].Z + t1.P[2].Z) / 3));

            foreach (var tri in toDraw)
            {
                VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);
                for (int j = 0; j < 3; j++)
                {
                    float x = (tri.P[j].X + 1) * window.Size.X / 2;
                    float y = (tri.P[j].Y + 1) * window.Size.Y / 2;
                    triangle[(uint)j] = new Vertex(new Vector2f(x, y), Color.Black);
                }
                window.Draw(triangle);
            }

            theta += 0.01f * (allowRotation ? 1 : 0);
            window.Display();
        }
    }
}