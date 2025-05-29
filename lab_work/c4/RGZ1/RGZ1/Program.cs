using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Wireframe3DViewer
{
    class Program
    {
        static void Main()
        {
            // Настройки окна и инициализация камеры
            const int WINDOW_WIDTH = 1920;
            const int WINDOW_HEIGHT = 1080;
            Vec4 cameraLoc = new Vec4(0, -2.0f, -5);
            Vec4 lookDir = new Vec4(0, 0, 1);
            Vec4 upDir = new Vec4(0, 1, 0);

            // Направление освещения (мировые координаты)
            Vec4 lightDirWorld = new Vec4(0.5f, 1, -0.5f);
            lightDirWorld.Normalize();

            // Создание окна и загрузка моделиwindow
            var window = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), "3D Engine");
            Vector2[] Points = new Vector2[8];
            Points[0] = new Vector2(0, 0);
            Points[1] = new Vector2(1, 0);
            Points[2] = new Vector2(0, 1);
            Points[3] = new Vector2(1, 1);
            Points[4] = new Vector2(2, 2);
            Points[5] = new Vector2(-5, 5);
            Points[6] = new Vector2(10, 0);
            Points[7] = new Vector2(0, 10);

            
            Mesh mesh = Mesh.Triangulate(Points);



            
            float thetaY = 0;
            float thetaX = 0;
            float thetaZ = 0;
            float scale = 1f;

            Vec4 Position_model = new Vec4(0, 0, 0);


            // Матрица проекции
            // Mat4x4 projMat = Mat4x4.GetProjectionMatrix(1920, 1080, 0.1f, 1000f, 90f);
            Mat4x4 projMat = Mat4x4.GetOrthographicProjectionMatrix(
                -WINDOW_WIDTH / 100, WINDOW_WIDTH / 100,
                -WINDOW_HEIGHT / 100, WINDOW_HEIGHT / 100,
                0.1f, 100.0f);


            ZBuffer zBuffer = new ZBuffer((uint)WINDOW_WIDTH, (uint)WINDOW_HEIGHT);


            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(Color.Black);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) window.Close();


                //Обработка перемещения фигуры
                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    Position_model.z += 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    Position_model.z -= 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    Position_model.x += 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    Position_model.x -= 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
                {
                    Position_model.y += 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                {
                    Position_model.y -= 0.1f;
                }


                //клавишами q/e повоачиваем модель по оси Z
                if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                {
                    thetaZ += 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                {
                    thetaZ -= 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Z))
                {
                    if (scale > 0.1f) scale -= 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.X))
                {
                    scale += 0.1f;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                {
                    Position_model = new Vec4();
                    thetaY = 0;
                    thetaX = 0;
                    thetaZ = 0;
                }

                //обработка двидение мыши
                Vector2i mouseOffset = new Vector2i(0, 0);
                Vector2i mousePos = Mouse.GetPosition(window);
                mouseOffset.X = mousePos.X - WINDOW_WIDTH / 2;
                mouseOffset.Y = mousePos.Y - WINDOW_HEIGHT / 2;
                Mouse.SetPosition(new Vector2i(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2), window);

                // в зависимости от того на сколько отклонилась мышь, меняем поворо модели
                thetaY += mouseOffset.X * 0.01f;
                thetaX += mouseOffset.Y * 0.01f;

                // Создание матриц преобразований
                Mat4x4 viewMat = Mat4x4.GetPointAtMatrix(cameraLoc, cameraLoc + lookDir, upDir);
                viewMat.Invert();
                Mat4x4 scaleMat = Mat4x4.GetScaleMatrix(scale, scale, scale);
                Mat4x4 rotationMat = Mat4x4.GetRotationY(thetaY) * Mat4x4.GetRotationZ(thetaZ) *
                                     Mat4x4.GetRotationX(thetaX);


                Mat4x4 translationMat =
                    Mat4x4.GetTranslationMatrix(Position_model.x, Position_model.y, Position_model.z);
                Mat4x4 modelMat = scaleMat * rotationMat * translationMat;

                // Преобразование направления света в view space
                Vec4 lightDirView = lightDirWorld * viewMat;
                lightDirView.Normalize();

                foreach (Triangle t in mesh.Triangles)
                {
                    Triangle viewSpaceTri = new Triangle();
                    Triangle projTri = new Triangle();

                    // Преобразование точек в пространство камеры
                    for (int k = 0; k < 3; k++)
                    {
                        viewSpaceTri.Points[k] = t.Points[k] * modelMat * viewMat;
                        projTri.Points[k] = viewSpaceTri.Points[k] * projMat;
                    }

                    // Вычисление нормали грани
                    Vec4 normal = Vec4.Cross(
                        viewSpaceTri.Points[1] - viewSpaceTri.Points[0],
                        viewSpaceTri.Points[2] - viewSpaceTri.Points[0]
                    );
                    normal.Normalize();

                    if (Vec4.Dot(normal, lookDir) > 0 || Vec4.Dot(normal, lookDir) > 0)
                    {
                        // Расчет интенсивности освещения
                        float intensity = Vec4.Dot(normal, lightDirView);
                        intensity = Math.Abs(intensity); // Math.Max(0, intensity)
                        float ambient = 0.2f;
                        intensity = ambient + intensity * (1 - ambient);

                        // Подготавливаем треугольник для Z-Buffer
                        Triangle screenTri = new Triangle();
                        for (int k = 0; k < 3; k++)
                        {
                            // Проекция в экранные координаты
                            Vector2f p = Project(projTri.Points[k], window.Size);
                            // Используем z из view space для глубины
                            screenTri.Points[k] = new Vec4(p.X, p.Y, viewSpaceTri.Points[k].z, 1);
                        }

                        // Добавляем треугольник в Z-Buffer
                        zBuffer.AddTriangle(screenTri, intensity);
                    }

                    
                    
                    // // Проверка видимости (нормаль направлена от камеры)
                    // if (Vec4.Dot(normal, lookDir) < 0)
                    //     continue;
                    //
                    // // Отрисовка видимой грани
                    // VertexArray outline = new VertexArray(PrimitiveType.Lines, 6);
                    // for (int j = 0; j < 3; j++)
                    // {
                    //     Vector2f p1 = Project(projTri.Points[j], window.Size);
                    //     Vector2f p2 = Project(projTri.Points[(j + 1) % 3], window.Size);
                    //     outline[(uint)(2 * j)] = new Vertex(p1, Color.White);
                    //     outline[(uint)(2 * j + 1)] = new Vertex(p2, Color.White);
                    // }
                    // window.Draw(outline);
                }

                zBuffer.FinalizeFrame();
                Sprite renderSprite = new Sprite(zBuffer.GetTexture());
                window.Draw(renderSprite);
                // zBuffer.Draw(window);
                zBuffer.Clear();

                window.Display();
            }
        }

        static Vector2f Project(Vec4 point, Vector2u windowSize)
        {
            float x = (point.x + 1) * windowSize.X / 2;
            float y = (1 - (point.y + 1) * 0.5f) * windowSize.Y;
            return new Vector2f(x, y);
        }
    }

    public class ZBuffer
    {
        private float[,] depthBuffer;
        private Image image;
        private Texture texture;
        private uint width;
        private uint height;

        public ZBuffer(uint width, uint height)
        {
            this.width = width;
            this.height = height;
            depthBuffer = new float[width, height];
            image = new Image(width, height);
            texture = new Texture(width, height);
            Clear();
        }

        public void Clear()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    depthBuffer[x, y] = float.MinValue;
                    image.SetPixel((uint)x, (uint)y, Color.Black);
                }
            }
        }

        public void Draw(RenderWindow window)
        {
            const int RECT_SIZE = 1; // Размер пикселя
            RectangleShape pixel = new RectangleShape(new Vector2f(RECT_SIZE, RECT_SIZE))
            {
                OutlineThickness = 0
            };

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    if (color == Color.Black) continue;

                    pixel.Position = new Vector2f(x, y);
                    pixel.FillColor = color;
                    window.Draw(pixel);
                }
            }
        }

        public void AddTriangle(Triangle tri, float intensity)
        {
            Vec4[] points = tri.Points;
            Vector2f[] screenPoints = new Vector2f[3];
            float[] depths = new float[3];

            // Конвертируем вершины в экранные координаты и глубину
            for (int i = 0; i < 3; i++)
            {
                screenPoints[i] = new Vector2f(points[i].x, points[i].y);
                depths[i] = points[i].z;
            }

            // Находим ограничивающий прямоугольник
            float minX = MathF.Max(0, MathF.Min(MathF.Min(screenPoints[0].X, screenPoints[1].X), screenPoints[2].X));
            float maxX = MathF.Min(width - 1,
                MathF.Max(MathF.Max(screenPoints[0].X, screenPoints[1].X), screenPoints[2].X));
            float minY = MathF.Max(0, MathF.Min(MathF.Min(screenPoints[0].Y, screenPoints[1].Y), screenPoints[2].Y));
            float maxY = MathF.Min(height - 1,
                MathF.Max(MathF.Max(screenPoints[0].Y, screenPoints[1].Y), screenPoints[2].Y));

            // Проход по всем пикселям в ограничивающем прямоугольнике
            for (int y = (int)minY; y <= maxY; y++)
            {
                for (int x = (int)minX; x <= maxX; x++)
                {
                    Vector2f p = new Vector2f(x, y);
                    Vec4 bary = Barycentric(screenPoints[0], screenPoints[1], screenPoints[2], p);

                    if (bary.x >= 0 && bary.y >= 0 && bary.z >= 0)
                    {
                        // Интерполируем глубину
                        float depth = bary.x * depths[0] + bary.y * depths[1] + bary.z * depths[2];

                        if (depth > depthBuffer[x, y])
                        {
                            depthBuffer[x, y] = depth;
                            image.SetPixel((uint)x, (uint)y, new Color( (byte)(intensity * 255), (byte)(intensity * 255), (byte)(intensity * 255)));
                        }
                    }
                }
            }
        }

        private Vec4 Barycentric(Vector2f a, Vector2f b, Vector2f c, Vector2f p)
        {
            Vector2f v0 = b - a;
            Vector2f v1 = c - a;
            Vector2f v2 = p - a;

            float denom = v0.X * v1.Y - v1.X * v0.Y;
            if (MathF.Abs(denom) < 1e-5) return new Vec4(-1, -1, -1);

            float v = (v2.X * v1.Y - v1.X * v2.Y) / denom;
            float w = (v0.X * v2.Y - v2.X * v0.Y) / denom;
            float u = 1.0f - v - w;

            return new Vec4(u, v, w);
        }

        public void FinalizeFrame()
        {
            texture.Update(image);
        }

        public Texture GetTexture()
        {
            return texture;
        }
    }
}