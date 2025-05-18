using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;


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

        // Создание окна и загрузка модели
        var window = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), "3D Engine");
        Mesh mesh = new Mesh();
        mesh.LoadFromFile("2.obj");

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

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(Color.Black);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) window.Close();


            //Обработка перемещения фигуры
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                Position_model.z += 0.001f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                Position_model.z -= 0.001f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                Position_model.x += 0.001f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                Position_model.x -= 0.001f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                Position_model.y += 0.001f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
            {
                Position_model.y -= 0.001f;
            }


            //клавишами q/e повоачиваем модель по оси Z
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                thetaZ += 0.001f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                thetaZ -= 0.001f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Z))
            {
                if (scale > 0.1f) scale -= 0.01f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.X))
            {
                scale += 0.01f;
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
            thetaY += mouseOffset.X * 0.05f;
            thetaX += mouseOffset.Y * 0.05f;

            // Создание матриц преобразований
            Mat4x4 viewMat = Mat4x4.GetPointAtMatrix(cameraLoc, cameraLoc + lookDir, upDir);
            viewMat.Invert();
            Mat4x4 scaleMat = Mat4x4.GetScaleMatrix(scale, scale, scale);
            Mat4x4 rotationMat = Mat4x4.GetRotationY(thetaY) * Mat4x4.GetRotationZ(thetaZ) *
                                 Mat4x4.GetRotationX(thetaX);


            Mat4x4 translationMat =
                Mat4x4.GetTranslationMatrix(Position_model.x, Position_model.y, Position_model.z);
            Mat4x4 modelMat = scaleMat * rotationMat * translationMat;

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


                // Проверка видимости (нормаль направлена от камеры)
                if (Vec4.Dot(normal, lookDir) < 0)
                    continue;

                // Отрисовка видимой грани
                VertexArray outline = new VertexArray(PrimitiveType.Lines, 6);
                for (int j = 0; j < 3; j++)
                {
                    Vector2f p1 = Project(projTri.Points[j], window.Size);
                    Vector2f p2 = Project(projTri.Points[(j + 1) % 3], window.Size);
                    outline[(uint)(2 * j)] = new Vertex(p1, Color.White);
                    outline[(uint)(2 * j + 1)] = new Vertex(p2, Color.White);
                    
                }

                window.Draw(outline);
            }

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