using System;
using System.IO;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Program
{
    const int WindowWidth = 800;
    const int WindowHeight = 600;
    const float RotationSpeed = 0.02f;

    static float angleX = MathF.PI*3;
    static float angleY = 0f;

    static void Main()
    {
        // Создание окна
        var window = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), "Метод плавающего горизонта");
        window.SetFramerateLimit(60);
        window.Closed += (sender, e) => window.Close();
        window.KeyPressed += OnKeyPressed;

        // Загрузка изображения
        var image = new Image("circles_m_200.bmp");
        uint width = image.Size.X;
        uint height = image.Size.Y;

        // Преобразование изображения в массив высот
        float[,] heightMap = new float[width, height];
        for (uint y = 0; y < height; y++)
        {
            for (uint x = 0; x < width; x++)
            {
                var pixel = image.GetPixel(x, y);
                // Преобразование яркости в диапазоне 0-255 к высоте
                float brightness = (pixel.R + pixel.G + pixel.B) / 10f;
                heightMap[x, y] = brightness;
            }
        }

        // Основной цикл
        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(Color.Black);

            // Массивы для хранения горизонтов
            float[] upperHorizon = new float[WindowWidth];
            float[] lowerHorizon = new float[WindowWidth];
            for (int i = 0; i < WindowWidth; i++)
            {
                upperHorizon[i] = float.MinValue;
                lowerHorizon[i] = float.MaxValue;
            }

            // Построение линий для каждой строки изображения
            Mat4x4 mat = Mat4x4.GetRotationX(angleX) *
                         Mat4x4.GetRotationY(angleY) *
                         Mat4x4.GetOrthographicProjectionMatrix(
                             -WindowWidth / 100, WindowWidth / 100,
                             -WindowHeight / 100, WindowHeight / 100,
                             0.1f, 100.0f);
            for (int y = 0; y < height; y++)
            {
                VertexArray line = new VertexArray(PrimitiveType.LinesStrip);
                for (int x = 0; x < width; x++)
                {
                    if (y % 5 != 0 ){continue;}
                    // if (x % 10 != 0 ){continue;}

                    // Получение высоты
                    float z = heightMap[x, y];
                    Vec4 p = new Vec4((x - width / 2f) * 10, (y- height / 2f)*10, z*5);
                    p *= mat ;
                    float screenX = p.x;
                    float screenY = p.y;
                    screenX += WindowWidth / 2f;
                    screenY += WindowHeight / 2f;
                    
                    // // Центрирование координат
                    // float centeredX = x - width / 2f;
                    // float centeredY = y - height / 2f;
                    //
                    // // Вращение по оси X
                    // float cosX = (float)Math.Cos(angleX);
                    // float sinX = (float)Math.Sin(angleX);
                    // float y1 = centeredY * cosX - z * sinX;
                    // float z1 = centeredY * sinX + z * cosX;
                    //
                    // // Вращение по оси Y
                    // float cosY = (float)Math.Cos(angleY);
                    // float sinY = (float)Math.Sin(angleY);
                    // float x1 = centeredX * cosY + z1 * sinY;
                    // float z2 = -centeredX * sinY + z1 * cosY;
                    //
                    // // Проекция на 2D-плоскость
                    // float screenX = x1 + WindowWidth / 2f;
                    // float screenY = y1 + WindowHeight / 2f;

                    int ix = (int)screenX;
                    if (ix >= 0 && ix < WindowWidth)
                    {
                        if (screenY > upperHorizon[ix] || screenY < lowerHorizon[ix])
                        {
                            line.Append(new Vertex(new Vector2f(screenX, screenY), Color.White));
                            upperHorizon[ix] = Math.Max(upperHorizon[ix], screenY);
                            lowerHorizon[ix] = Math.Min(lowerHorizon[ix], screenY);
                        }
                        else
                        {
                            line.Append(new Vertex(new Vector2f(screenX, screenY), Color.Transparent));
                        }
                    }
                }

                window.Draw(line);
            }

            window.Display();
        }
    }

    static void OnKeyPressed(object sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Left)
            angleY -= RotationSpeed;
        else if (e.Code == Keyboard.Key.Right)
            angleY += RotationSpeed;
        else if (e.Code == Keyboard.Key.Up)
            angleX -= RotationSpeed;
        else if (e.Code == Keyboard.Key.Down)
            angleX += RotationSpeed;
    }
}