using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Program
{
    static void Main()
    {
        // Инициализация камеры и проекции
        Vec4 cameraLoc = new Vec4(0, -2.0f, -5);
        Mat4x4 projMat = Mat4x4.GetProjectionMatrix(1920, 1080, 0.1f, 1000f, 90f);

        // Загрузка модели
        Mesh testMesh = new Mesh();
        // if (!testMesh.LoadFromFile("2.obj"))
        // {
        //     Console.WriteLine("Failed to load model");
        //     return;
        // }
        testMesh.DefineAsCube();
        // Настройки приложения
        float thetaY = 0;
        float thetaX = 0;
        float thetaZ = 0;
        bool outlineOnly = false;
        bool allowMouseMovement = false;
        bool allowRotation = false;
        bool[] keys = new bool[6];

        // Направления
        Vec4 lookDir = new Vec4(0, 0, 1, 0);
        Vec4 upDir = new Vec4(0, 1, 0, 0);
        Vec4 lightDir = new Vec4(1, -0.5f, -0.7f, 0);
        lightDir.Normalize();

        // Таймеры
        Clock movementClock = new Clock();
        Clock fpsClock = new Clock();

        // Создание окна
        const int windowHeight = 1080;
        const int windowWidth = 1920;
        RenderWindow window = new RenderWindow(new VideoMode((uint)windowWidth, (uint)windowHeight), "3D Engine");
        window.SetVerticalSyncEnabled(true);
        Mouse.SetPosition(new Vector2i(windowWidth / 2, windowHeight / 2), window);
        window.SetMouseCursorVisible(false);
        allowMouseMovement = true;

        // Главный цикл
        while (window.IsOpen)
        {
            window.DispatchEvents();

            // Обработка ввода
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) window.Close();
            keys[0] = Keyboard.IsKeyPressed(Keyboard.Key.W);
            keys[1] = Keyboard.IsKeyPressed(Keyboard.Key.A);
            keys[2] = Keyboard.IsKeyPressed(Keyboard.Key.S);
            keys[3] = Keyboard.IsKeyPressed(Keyboard.Key.D);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                thetaY += 0.1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                thetaY -= 0.1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Z))
            {
                thetaX += 0.1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.X))
            {
                thetaX -= 0.1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                thetaZ += 0.1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.T))
            {
                thetaZ -= 0.1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
            {
                Vec4 t1 = testMesh.Tris[0].Points[0];

                foreach (Triangle t in testMesh.Tris)
                {
                    t.Points[0] *= Mat4x4.GetMatchtabe(1.1f, 1.1f,1.1f, t1);
                    t.Points[1] *= Mat4x4.GetMatchtabe(1.1f, 1.1f,1.1f, t1);
                    t.Points[2] *= Mat4x4.GetMatchtabe(1.1f, 1.1f,1.1f, t1);
                }
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.V))
            {
                Vec4 t1 = testMesh.Tris[0].Points[0];

                foreach (Triangle t in testMesh.Tris)
                {
                    t.Points[0] *= Mat4x4.GetMatchtabe(0.9f, 0.9f,0.9f, t1);
                    t.Points[1] *= Mat4x4.GetMatchtabe(0.9f, 0.9f,0.9f, t1);
                    t.Points[2] *= Mat4x4.GetMatchtabe(0.9f, 0.9f,0.9f, t1);
                }
            }
            keys[4] = Keyboard.IsKeyPressed(Keyboard.Key.Space);
            keys[5] = Keyboard.IsKeyPressed(Keyboard.Key.LShift);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Tab)) outlineOnly = !outlineOnly;
            if (Keyboard.IsKeyPressed(Keyboard.Key.LControl)) allowMouseMovement = !allowMouseMovement;
            if (Keyboard.IsKeyPressed(Keyboard.Key.R)) allowRotation = !allowRotation;

            // Обработка движения мыши
            Vector2i mouseOffset = new Vector2i(0, 0);
            if (allowMouseMovement)
            {
                Vector2i mousePos = Mouse.GetPosition(window);
                mouseOffset.X = mousePos.X - windowWidth / 2;
                mouseOffset.Y = mousePos.Y - windowHeight / 2;
                Mouse.SetPosition(new Vector2i(windowWidth / 2, windowHeight / 2), window);
            }

            // Движение камеры
            if (movementClock.ElapsedTime.AsMilliseconds() >= 10)
            {
                Vec4 vel = new Vec4(0, 0, 0, 0);
                if (keys[0]) vel += new Vec4(0, 0, 0.1f, 0);
                if (keys[1]) vel += new Vec4(-0.1f, 0, 0, 0);
                if (keys[2]) vel += new Vec4(0, 0, -0.1f, 0);
                if (keys[3]) vel += new Vec4(0.1f, 0, 0, 0);
                if (keys[4]) vel += new Vec4(0, -0.1f, 0, 0);
                if (keys[5]) vel += new Vec4(0, 0.1f, 0, 0);

                foreach (Triangle t in testMesh.Tris)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        t.Points[i] += vel;
                    }
                }


                // Vec4 tempDir = new Vec4(lookDir.x, 0, lookDir.z, 0);
                // tempDir.Normalize();
                // float phi = MathF.Acos(Vec4.Dot(tempDir, new Vec4(0, 0, 1, 0)));
                // phi = tempDir.x < 0 ? phi : -phi;
                // Vec4 rotVel = vel * Mat4x4.GetRotationY(phi);
                // cameraLoc += rotVel;
                movementClock.Restart();
            }

            // Обновление направления взгляда
            lookDir = lookDir * Mat4x4.GetRotationY(-mouseOffset.X * 0.005f);
            Vec4 horDir = new Vec4(lookDir.x, 0, lookDir.z, 0);
            horDir.Normalize();
            float phi2 = MathF.Acos(Vec4.Dot(horDir, new Vec4(0, 0, 1, 0)));
            phi2 = horDir.x < 0 ? -phi2 : phi2;
            Vec4 tempDir2 = lookDir * Mat4x4.GetRotationY(phi2);
            tempDir2 = tempDir2 * Mat4x4.GetRotationX(-mouseOffset.Y * 0.005f);
            lookDir = tempDir2 * Mat4x4.GetRotationY(-phi2);
            lookDir.Normalize();

            // Матрица вида
            Vec4 target = cameraLoc + lookDir;
            Mat4x4 viewMat = Mat4x4.GetPointAtMatrix(cameraLoc, target, upDir);
            viewMat.Invert();

            // Очистка экрана
            window.Clear(Color.White);

            // Обработка треугольников
            List<Triangle> toDraw = new List<Triangle>();
            foreach (Triangle t in testMesh.Tris)
            {
                // Преобразование модели
                Triangle newTri = new Triangle(
                    t.Points[0] * Mat4x4.GetRotationY(thetaY * 0.5f) * Mat4x4.GetRotationX(thetaX * 0.5f) *
                    Mat4x4.GetRotationZ(thetaZ * 0.5f),
                    t.Points[1] * Mat4x4.GetRotationY(thetaY * 0.5f) * Mat4x4.GetRotationX(thetaX * 0.5f) *
                    Mat4x4.GetRotationZ(thetaZ * 0.5f),
                    t.Points[2] * Mat4x4.GetRotationY(thetaY * 0.5f) * Mat4x4.GetRotationX(thetaX * 0.5f) *
                    Mat4x4.GetRotationZ(thetaZ * 0.5f)
                );

                // Расчет нормали
                Vec4 a = Vec4.Cross(newTri.Points[1] - newTri.Points[0], newTri.Points[2] - newTri.Points[0]);
                a.Normalize();
                newTri.Normal = a;

                // Отсечение невидимых граней
                Vec4 camDir = (newTri.Points[0] + newTri.Points[1] + newTri.Points[2]) / 3 - cameraLoc;
                if (Vec4.Dot(newTri.Normal, camDir) >= 0)
                    continue;

                // Преобразование в пространство вида
                for (int k = 0; k < 3; k++)
                {
                    newTri.Points[k] = newTri.Points[k] * viewMat;
                }

                // Клиппинг у ближней плоскости
                List<Triangle> clipped = newTri.ClipAgainstPlane(new Vec4(0, 0, 0.2f), new Vec4(0, 0, 1));
                foreach (Triangle clippedTri in clipped)
                {
                    Triangle projected = new Triangle(
                        clippedTri.Points[0] * projMat,
                        clippedTri.Points[1] * projMat,
                        clippedTri.Points[2] * projMat
                    );
                    projected.Normal = clippedTri.Normal;
                    toDraw.Add(projected);
                }
            }

            // Дополнительный клиппинг по границам экрана
            List<Triangle> finalTriangles = new List<Triangle>();
            foreach (Triangle t in toDraw)
            {
                List<Triangle> temp = new List<Triangle> { t };

                for (int plane = 0; plane < 4; plane++)
                {
                    List<Triangle> toAdd = new List<Triangle>();
                    foreach (Triangle tri in temp)
                    {
                        switch (plane)
                        {
                            case 0: // TOP
                                toAdd.AddRange(tri.ClipAgainstPlane(new Vec4(0, -1, 0), new Vec4(0, 1, 0)));
                                break;
                            case 1: // BOTTOM
                                toAdd.AddRange(tri.ClipAgainstPlane(new Vec4(0, 1, 0), new Vec4(0, -1, 0)));
                                break;
                            case 2: // LEFT
                                toAdd.AddRange(tri.ClipAgainstPlane(new Vec4(-1, 0, 0), new Vec4(1, 0, 0)));
                                break;
                            case 3: // RIGHT
                                toAdd.AddRange(tri.ClipAgainstPlane(new Vec4(1, 0, 0), new Vec4(-1, 0, 0)));
                                break;
                        }
                    }

                    temp = toAdd;
                }

                finalTriangles.AddRange(temp);
            }

            // Сортировка по глубине
            finalTriangles = finalTriangles.OrderByDescending(t =>
                (t.Points[0].z + t.Points[1].z + t.Points[2].z) / 3).ToList();

            // Отрисовка
            foreach (Triangle T in finalTriangles)
            {
                if (outlineOnly)
                {
                    VertexArray outline = new VertexArray(PrimitiveType.LineStrip, 4);
                    for (int j = 0; j < 4; j++)
                    {
                        float x = (T.Points[j % 3].x + 1) * window.Size.X / 2;
                        float y = (T.Points[j % 3].y + 1) * window.Size.Y / 2;
                        outline[(uint)j] = new Vertex(new Vector2f(x, y), Color.Black);
                    }

                    window.Draw(outline);
                }
                else
                {
                    VertexArray tri = new VertexArray(PrimitiveType.Triangles, 3);
                    for (int j = 0; j < 3; j++)
                    {
                        float x = (T.Points[j].x + 1) * window.Size.X / 2;
                        float y = (T.Points[j].y + 1) * window.Size.Y / 2;
                        float light = Math.Max(0, Vec4.Dot(-T.Normal, lightDir));
                        byte R = (byte)(153 * (0.3f + 0.7f * light));
                        byte G = (byte)(249 * (0.3f + 0.7f * light));
                        byte B = (byte)(142 * (0.3f + 0.7f * light));
                        tri[(uint)j] = new Vertex(new Vector2f(x, y), new Color(R, G, B));
                    }

                    window.Draw(tri);
                }
            }

            // Обновление FPS
            int fps = (int)(1.0f / fpsClock.ElapsedTime.AsSeconds());
            fpsClock.Restart();
            window.SetTitle($"FPS: {fps}; Triangles: {testMesh.Tris.Count}; Visible: {finalTriangles.Count}");

            if (allowRotation) thetaY += 0.01f;
            window.Display();
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Numerics;
// using SFML.Graphics;
// using SFML.System;
// using SFML.Window;
//
// class Program
// {
//     static void Main()
//     {
//         // Инициализация камеры и проекции
//         Vec4 cameraLoc = new Vec4(0, -2.0f, -5);
//         // Matrix4x4 projMat = Matrix4x4.CreatePerspectiveFieldOfView(
//         //     MathF.PI / 2, // 90 градусов в радианах
//         //     1920f / 1080f,
//         //     0.1f,
//         //     1000f
//         // );
//         
//         float fovRad = 90 * MathF.PI / 180;
//         float aspect = 1920f / 1080f;
//         float fNear = 0.1f;
//         float fFar = 1000f;
//
//         Matrix4x4 projMat = new Matrix4x4(
//             (aspect)/MathF.Tan(fovRad/2), 0, 0, 0,
//             0, 1/MathF.Tan(fovRad/2), 0, 0,
//             0, 0, fFar/(fFar-fNear), 1,
//             0, 0, -fFar*fNear/(fFar-fNear), 0
//         );
//
//         // Загрузка модели
//         Mesh testMesh = new Mesh();
//         // if (!testMesh.LoadFromFile("cube.obj"))
//         // {
//         //     Console.WriteLine("Failed to load model");
//         //     return;
//         // }
//         
//         testMesh.DefineAsCube();
//
//         // Настройки приложения
//         float theta = 0;
//         bool outlineOnly = false;
//         bool allowMouseMovement = false;
//         bool allowRotation = true;
//         bool[] keys = new bool[6];
//
//         // Направления
//         Vec4 lookDir = new Vec4(0, 0, 1);
//         Vec4 upDir = new Vec4(0, 1, 0);
//         Vec4 lightDir = Vec4.Normalize(new Vec4(1, -0.5f, -0.7f));
//
//         // Таймеры
//         Clock movementClock = new Clock();
//         Clock fpsClock = new Clock();
//
//         // Создание окна
//         const int windowHeight = 1080;
//         const int windowWidth = 1920;
//         
//         SFML.System.Vector2i mouseOffset = new SFML.System.Vector2i(0, 0);
//         
//         RenderWindow window = new RenderWindow(new VideoMode((uint)windowWidth, (uint)windowHeight), "3D Engine");
//         window.SetVerticalSyncEnabled(true);
//         Mouse.SetPosition(new Vector2i(windowWidth / 2, windowHeight / 2), window);
//         window.SetMouseCursorVisible(false);
//         allowMouseMovement = true;
//
//         // Главный цикл
//         while (window.IsOpen)
//         {
//             window.DispatchEvents();
//
//             // Обработка событий
//             if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) window.Close();
//             keys[0] = Keyboard.IsKeyPressed(Keyboard.Key.W);
//             keys[1] = Keyboard.IsKeyPressed(Keyboard.Key.A);
//             keys[2] = Keyboard.IsKeyPressed(Keyboard.Key.S);
//             keys[3] = Keyboard.IsKeyPressed(Keyboard.Key.D);
//             keys[4] = Keyboard.IsKeyPressed(Keyboard.Key.Space);
//             keys[5] = Keyboard.IsKeyPressed(Keyboard.Key.LShift);
//
//             if (Keyboard.IsKeyPressed(Keyboard.Key.Tab)) outlineOnly = !outlineOnly;
//             if (Keyboard.IsKeyPressed(Keyboard.Key.LControl)) allowMouseMovement = !allowMouseMovement;
//             if (Keyboard.IsKeyPressed(Keyboard.Key.R)) allowRotation = !allowRotation;
//
//             // Обработка движения мыши
//             if (allowMouseMovement)
//             {
//                 SFML.System.Vector2i mousePos = Mouse.GetPosition(window);
//                 mouseOffset.X = mousePos.X - windowWidth / 2;
//                 mouseOffset.Y = mousePos.Y - windowHeight / 2;
//                 Mouse.SetPosition(new SFML.System.Vector2i(windowWidth / 2, windowHeight / 2), window);
//             }
//
//             // Обработка движения камеры
//             if (movementClock.ElapsedTime.AsMilliseconds() >= 10)
//             {
//                 Vec4 vel = new Vec4(0, 0, 0);
//                 if (keys[0]) vel += new Vec4(0, 0, 0.1f);
//                 if (keys[1]) vel += new Vec4(-0.1f, 0, 0);
//                 if (keys[2]) vel += new Vec4(0, 0, -0.1f);
//                 if (keys[3]) vel += new Vec4(0.1f, 0, 0);
//                 if (keys[4]) vel += new Vec4(0, -0.1f, 0);
//                 if (keys[5]) vel += new Vec4(0, 0.1f, 0);
//
//                 Vec4 tempDir = new Vec4(lookDir.x, 0, lookDir.z);
//                 tempDir.Normalize();
//                 float phi = MathF.Acos(Vec4.Dot(tempDir, new Vec4(0, 0, 1)));
//                 phi = tempDir.x < 0 ? phi : -phi;
//                 Vec4 rotVel = Vec4.Transform(vel, Matrix4x4.CreateRotationY(phi));
//                 cameraLoc += rotVel;
//                 movementClock.Restart();
//             }
//
//             // Обновление направления взгляда
//             lookDir = Vec4.Transform(lookDir, Matrix4x4.CreateRotationY(-mouseOffset.X * 0.005f));
//             Vec4 horDir = new Vec4(lookDir.X, 0, lookDir.Z);
//             horDir = Vec4.Normalize(horDir);
//             float phi2 = MathF.Acos(Vec4.Dot(horDir, new Vec4(0, 0, 1)));
//             phi2 = horDir.X < 0 ? -phi2 : phi2;
//             Vec4 tempDir2 = Vec4.Transform(lookDir, Matrix4x4.CreateRotationY(phi2));
//             tempDir2 = Vec4.Transform(tempDir2, Matrix4x4.CreateRotationX(-mouseOffset.Y * 0.005f));
//             lookDir = Vec4.Transform(tempDir2, Matrix4x4.CreateRotationY(-phi2));
//             lookDir = Vec4.Normalize(lookDir);
//
//             // Матрица вида
//             Vec4 target = cameraLoc + lookDir;
//             Matrix4x4 viewMat = Matrix4x4.CreateLookAt(cameraLoc, target, upDir);
//             Matrix4x4.Invert(viewMat, out viewMat);
//
//             // Очистка экрана
//             window.Clear(Color.White);
//
//             // Обработка треугольников
//             List<Triangle> toDraw = new List<Triangle>();
//             foreach (Triangle t in testMesh.Tris)
//             {
//                 Triangle newTri = new Triangle();
//                 for (int k = 0; k < 3; k++)
//                 {
//                     if (t.Points[k] == null) continue;
//                     newTri.Points[k] = Vec4.Transform(t.Points[k], Matrix4x4.CreateRotationY(theta * 1.5f));
//                     newTri.Points[k] = Vec4.Transform(newTri.Points[k], Matrix4x4.CreateRotationZ(MathF.PI));
//                 }
//
//                 newTri.Normal = Vec4.Normalize(Vec4.Cross(
//                     newTri.Points[2] - newTri.Points[0], 
//                     newTri.Points[1] - newTri.Points[0]));
//
//                 Vec4 camDir = (newTri.Points[0] + newTri.Points[1] + newTri.Points[2]) / 3 - cameraLoc;
//                 if (Vec4.Dot(newTri.Normal, camDir) < 0)
//                     continue;
//
//                 // Преобразование в пространство вида
//                 for (int k = 0; k < 3; k++)
//                 {
//                     newTri.Points[k] = Vec4.Transform(newTri.Points[k], viewMat);
//                 }
//
//                 // Клиппинг
//                 List<Triangle> clipped = newTri.ClipAgainstPlane(new Vec4(0, 0, 0.2f), new Vec4(0, 0, 1));
//                 foreach (Triangle clippedTri in clipped)
//                 {
//                     Triangle projected = new Triangle();
//                     for (int m = 0; m < 3; m++)
//                     {
//                         // Проекция
//                         Vector4 point = Vector4.Transform(
//                             new Vector4(clippedTri.Points[m], 1), 
//                             projMat);
//                         point /= point.W;
//                         projected.Points[m] = new Vec4(point.X, point.Y, point.Z);
//                     }
//                     projected.Normal = clippedTri.Normal;
//                     toDraw.Add(projected);
//                 }
//             }
//
//             // Сортировка по глубине
//             // toDraw = toDraw.OrderByDescending(t => 
//             //     (t.Points[0].Z + t.Points[1].Z + t.Points[2].Z) / 3).ToList();
//
//             // Отрисовка
//             foreach (Triangle T in toDraw)
//             {
//                 if (outlineOnly)
//                 {
//                     VertexArray outline = new VertexArray(PrimitiveType.LineStrip, 4);
//                     for (int j = 0; j < 4; j++)
//                     {
//                         float x = (T.Points[j % 3].X + 1) * window.Size.X / 2;
//                         float y = (T.Points[j % 3].Y + 1) * window.Size.Y / 2;
//                         outline[(uint)j] = new Vertex(
//                             new Vector2f(x, y), 
//                             Color.Black);
//                     }
//                     window.Draw(outline);
//                 }
//                 else
//                 {
//                     VertexArray tri = new VertexArray(PrimitiveType.Triangles, 3);
//                     for (int j = 0; j < 3; j++)
//                     {
//                         float x = (T.Points[j].X + 1) * window.Size.X / 2;
//                         float y = (T.Points[j].Y + 1) * window.Size.Y / 2;
//                         float light = Math.Max(0, Vec4.Dot(-T.Normal, lightDir));
//                         byte R = (byte)(153 * (0.3f + 0.7f * light));
//                         byte G = (byte)(249 * (0.3f + 0.7f * light));
//                         byte B = (byte)(142 * (0.3f + 0.7f * light));
//                         tri[(uint)j] = new Vertex(
//                             new Vector2f(x, y), 
//                             new Color(R, G, B));
//                     }
//                     window.Draw(tri);
//                 }
//             }
//
//             // Обновление FPS
//             int fps = (int)(1.0f / fpsClock.ElapsedTime.AsSeconds());
//             fpsClock.Restart();
//             window.SetTitle($"FPS: {fps}; Triangles rendering: {testMesh.Tris.Count}; Triangles drawing: {toDraw.Count}w, pos {cameraLoc}, dir {lookDir}");
//
//             if (allowRotation) theta += 0.01f;
//             
//             window.Display();
//         }
//     }
// }
//
//


// using System.Numerics;
//
//
// class Program
// {
//     static void Main()
//     {
//         const int WINDOW_HEIGHT = 1080;
//         const int WINDOW_WIDHT = 1920;
//             
//         
//         Vec4 camers_loc = new Vec4(0, -2, -5);
//         Matrix4x4 proj_mat = Matrix4x4.CreatePerspectiveFieldOfView(
//                 fieldOfView: 90, // FOV в радианах
//                 aspectRatio: WINDOW_WIDHT / WINDOW_HEIGHT,
//                 nearPlaneDistance: 0.1f,
//                 farPlaneDistance: 1000
//             );
//         Mesh mesh = new Mesh();
//         if (!mesh.LoadFromFile("cube.obj"))
//         {
//             mesh.DefineAsCube();
//             Console.WriteLine("can`t open file *.obj");
//         }
//         
//         float the
//     }
// }