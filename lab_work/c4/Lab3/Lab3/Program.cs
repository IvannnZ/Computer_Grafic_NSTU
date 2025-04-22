using System;
using System.Numerics;
using SFML.Graphics;
using SFML.System;  // Добавляем пространство имён для Vector2f
using SFML.Window;

namespace FloatingHorizonSFML
{
    class Program
    {
        static float[,]? heightMap; // Делаем поле nullable
        static Matrix4x4 rotation = Matrix4x4.Identity;
        static Vector2i lastMousePos; // Используем Vector2i из SFML
        const int scale = 2;

        private const int windowWidth = 1200;
        private const int windowHeight = 1000;
        static void Main(string[] args)
        {
            var window = new RenderWindow(
                new VideoMode(windowWidth, windowHeight), 
                "Lab3",
                Styles.Default);
            
            window.SetVerticalSyncEnabled(true);
            window.Closed += (s, e) => window.Close();
            
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

        static float[,] LoadHeightMap(string path)
        {
            using var image = new Image(path);
            var map = new float[image.Size.X, image.Size.Y];
            
            for (uint x = 0; x < image.Size.X; ++x)
                for (uint y = 0; y < image.Size.Y; ++y)
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

            
    
            var vertices = new VertexArray();
            int width = heightMap.GetLength(0);
            int heightMapHeight = heightMap.GetLength(1);

            for (int z = 0; z < heightMapHeight; z++)
            {
                var upperHorizon = new float[windowWidth];
                var lowerHorizon = new float[windowWidth];
                Array.Fill(upperHorizon, float.MinValue);
                Array.Fill(lowerHorizon, float.MaxValue);

                for (int x = 0; x < width; x++)
                {
                    var pos = Vector3.Transform(
                        new Vector3(x - width/2, heightMap[x,z] * 50, z - heightMapHeight/2),
                        rotation
                    );

                    // Ограничиваем координаты в пределах окна
                    int screenX = Math.Clamp(
                        (int)(pos.X * scale) + windowWidth/2, 
                        0, 
                        windowWidth - 1
                    );
            
                    int screenY = Math.Clamp(
                        (int)(-pos.Y * scale) + windowHeight/2,
                        0,
                        windowHeight - 1
                    );

                    if (screenY > upperHorizon[screenX])
                    {
                        upperHorizon[screenX] = screenY;
                        vertices.Append(new Vertex(
                            new Vector2f(screenX, screenY), 
                            Color.Black
                        ));
                    }
            
                    if (screenY < lowerHorizon[screenX])
                    {
                        lowerHorizon[screenX] = screenY;
                        vertices.Append(new Vertex(
                            new Vector2f(screenX, screenY), 
                            Color.Black
                        ));
                    }
                }
            }

            window.Draw(vertices);
        }
    }
}