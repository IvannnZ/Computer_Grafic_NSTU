using System;
using System.Numerics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace FloatingHorizonSFML
{
    class Program
    {
        static float[,]? heightMap;
        static Matrix4x4 rotation = Matrix4x4.Identity;
        static Vector2i lastMousePos;
        static float scale = 2.0f;
        const float scaleStep = 0.1f;
        const int windowWidth = 800;
        const int windowHeight = 600;

        static void Main(string[] args)
        {
            var window = new RenderWindow(
                new VideoMode(windowWidth, windowHeight),
                "Floating Horizon",
                Styles.Default);

            window.SetVerticalSyncEnabled(true);
            window.Closed += (s, e) => window.Close();
            window.MouseWheelScrolled += OnMouseWheelScrolled;

            heightMap = LoadHeightMap("circles_m_200.bmp");

            while (window.IsOpen)
            {
                window.DispatchEvents();
                HandleInput(window);

                window.Clear(Color.White);
                DrawSurface(window);
                window.Display();
            }
        }

        static void OnMouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            if (e.Delta > 0)
                scale += scaleStep;
            else if (e.Delta < 0)
                scale = Math.Max(0.1f, scale - scaleStep);
        }

        static float[,] LoadHeightMap(string path)
        {
            using var image = new Image(path);
            var map = new float[image.Size.X, image.Size.Y];

            for (uint x = 0; x < image.Size.X; x++)
                for (uint y = 0; y < image.Size.Y; y++)
                    map[x, y] = image.GetPixel(x, y).G / 255f;

            return map;
        }

        static void HandleInput(RenderWindow window)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                var currentPos = Mouse.GetPosition(window);
                var delta = currentPos - lastMousePos;

                rotation *= Matrix4x4.CreateRotationX(delta.Y * 0.01f)
                          * Matrix4x4.CreateRotationY(delta.X * 0.01f);

                lastMousePos = currentPos;
            }
            else
            {
                lastMousePos = Mouse.GetPosition(window);
            }
        }

        static void DrawSurface(RenderWindow window)
        {
            if (heightMap == null) return;

            var vertices = new VertexArray(PrimitiveType.Lines);
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            float[] upperHorizon = new float[windowWidth];
            float[] lowerHorizon = new float[windowWidth];
            Array.Fill(upperHorizon, float.MinValue);
            Array.Fill(lowerHorizon, float.MaxValue);

            // Первый проход: линии вдоль оси X
            for (int z = 0; z < height; z++)
            {
                float[] currentUpper = new float[windowWidth];
                float[] currentLower = new float[windowWidth];
                Array.Fill(currentUpper, float.MinValue);
                Array.Fill(currentLower, float.MaxValue);

                Vector2i? prevPoint = null;

                for (int x = 0; x < width; x++)
                {
                    var pos = Vector3.Transform(
                        new Vector3(x - width/2, heightMap[x,z] * 50, z - height/2),
                        rotation
                    );

                    int screenX = Math.Clamp((int)(pos.X * scale) + windowWidth/2, 0, windowWidth-1);
                    int screenY = Math.Clamp((int)(-pos.Y * scale) + windowHeight/2, 0, windowHeight-1);

                    bool isVisible = false;
                    
                    if (screenY > upperHorizon[screenX])
                    {
                        upperHorizon[screenX] = screenY;
                        currentUpper[screenX] = screenY;
                        isVisible = true;
                    }
                    if (screenY < lowerHorizon[screenX])
                    {
                        lowerHorizon[screenX] = screenY;
                        currentLower[screenX] = screenY;
                        isVisible = true;
                    }

                    if (prevPoint.HasValue && isVisible)
                    {
                        vertices.Append(new Vertex(new Vector2f(prevPoint.Value.X, prevPoint.Value.Y), Color.Black));
                        vertices.Append(new Vertex(new Vector2f(screenX, screenY), Color.Black));
                    }

                    prevPoint = isVisible ? new Vector2i(screenX, screenY) : null;
                }
            }

            // Второй проход: линии вдоль оси Z
            for (int x = 0; x < width; x++)
            {
                float[] currentUpper = new float[windowWidth];
                float[] currentLower = new float[windowWidth];
                Array.Fill(currentUpper, float.MinValue);
                Array.Fill(currentLower, float.MaxValue);

                Vector2i? prevPoint = null;

                for (int z = 0; z < height; z++)
                {
                    var pos = Vector3.Transform(
                        new Vector3(x - width/2, heightMap[x,z] * 50, z - height/2),
                        rotation
                    );

                    int screenX = Math.Clamp((int)(pos.X * scale) + windowWidth/2, 0, windowWidth-1);
                    int screenY = Math.Clamp((int)(-pos.Y * scale) + windowHeight/2, 0, windowHeight-1);

                    bool isVisible = false;
                    
                    if (screenY > upperHorizon[screenX])
                    {
                        upperHorizon[screenX] = screenY;
                        currentUpper[screenX] = screenY;
                        isVisible = true;
                    }
                    if (screenY < lowerHorizon[screenX])
                    {
                        lowerHorizon[screenX] = screenY;
                        currentLower[screenX] = screenY;
                        isVisible = true;
                    }

                    if (prevPoint.HasValue && isVisible)
                    {
                        vertices.Append(new Vertex(new Vector2f(prevPoint.Value.X, prevPoint.Value.Y), Color.Black));
                        vertices.Append(new Vertex(new Vector2f(screenX, screenY), Color.Black));
                    }

                    prevPoint = isVisible ? new Vector2i(screenX, screenY) : null;
                }
            }

            window.Draw(vertices);
        }
    }
}