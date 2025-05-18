using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

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

            // Создание окна и загрузка модели
            var window = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), "3D Engine");
            Mesh mesh = new Mesh();
            mesh.LoadFromFile("2.obj");

            float thetaY = 0;
            float thetaX = 0;
            float thetaZ = 0;
            float scale = 1f;

            Vec4 Position_model = new Vec4(0,0,0);


            // Матрица проекции
            // Mat4x4 projMat = Mat4x4.GetProjectionMatrix(1920, 1080, 0.1f, 1000f, 90f);
            Mat4x4 projMat = Mat4x4.GetOrthographicProjectionMatrix(
                -WINDOW_WIDTH/100, WINDOW_WIDTH/100, 
                -WINDOW_HEIGHT/100, WINDOW_HEIGHT/100, 
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

    // Классы Vec4, Mat4x4, Mesh и Triangle остаются без изменений
    // (убедитесь, что в них реализованы методы Cross, Normalize, преобразования матриц)
}


// using SFML.Graphics;
// using SFML.System;
// using SFML.Window;
// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace Wireframe3DViewer
// {
//     class Program
//     {
//         static void Main()
//         {
//             // Настройки окна
//             const int WINDOW_WIDTH = 1920;
//             const int WINDOW_HEIGHT = 1080;
//
//             // Инициализация камеры
//             Vec4 cameraLoc = new Vec4(0, -2.0f, -5);
//             Vec4 lookDir = new Vec4(0, 0, 1);
//             Vec4 upDir = new Vec4(0, 1, 0);
//
//
//             // Создание окна
//             var window = new RenderWindow(new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), "3D Engine");
//             window.SetVerticalSyncEnabled(true);
//             window.SetMouseCursorVisible(false);
//             Mouse.SetPosition(new Vector2i((int)WINDOW_WIDTH / 2, (int)WINDOW_HEIGHT / 2), window);
//
//             // Загрузка модели
//             Mesh mesh = new Mesh();
//             if (!mesh.LoadFromFile("1.obj"))
//             {
//                 Console.WriteLine("Failed to load model");
//                 return;
//             }
//
//             // mesh.DefineAsCube();
//
//
//             // параметры модели
//             float thetaY = 0;
//             float thetaX = 0;
//             float thetaZ = 0;
//             float scale = 1f;
//             
//             Vec4 Position_model = new Vec4();
//
//
//             // Матрица проекции
//             Mat4x4 projMat = Mat4x4.GetProjectionMatrix(1920, 1080, 0.1f, 1000f, 90f);
//
//
//             // Главный цикл
//             while (window.IsOpen)
//             {
//                 window.DispatchEvents();
//                 window.Clear(Color.Black);
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) window.Close();
//
//
//                 //Обработка перемещения фигуры
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.W))
//                 {
//                     Position_model.z += 0.1f;
//                 }
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.S))
//                 {
//                     Position_model.z -= 0.1f;
//                 }
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.D))
//                 {
//                     Position_model.x += 0.1f;
//                 }
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.A))
//                 {
//                     Position_model.x -= 0.1f;
//                 }
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
//                 {
//                     Position_model.y += 0.1f;
//                 }
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
//                 {
//                     Position_model.y -= 0.1f;
//                 }
//
//
//                 //клавишами q/e повоачиваем модель по оси Z
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
//                 {
//                     thetaZ += 0.1f;
//                 }
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.E))
//                 {
//                     thetaZ -= 0.1f;
//                 }
//                 
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.Z))
//                 {
//                     if (scale>0.1f) scale -= 0.01f;
//                 }
//
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.X))
//                 {
//                     scale += 0.01f;
//                 }
//                 
//                 if (Keyboard.IsKeyPressed(Keyboard.Key.R))
//                 {
//                     Position_model = new Vec4();
//                     thetaY = 0;
//                     thetaX = 0;
//                     thetaZ = 0;
//                 }
//
//                 //обработка двидение мыши
//                 Vector2i mouseOffset = new Vector2i(0, 0);
//                 Vector2i mousePos = Mouse.GetPosition(window);
//                 mouseOffset.X = mousePos.X - WINDOW_WIDTH / 2;
//                 mouseOffset.Y = mousePos.Y - WINDOW_HEIGHT / 2;
//                 Mouse.SetPosition(new Vector2i(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2), window);
//
//                 // в зависимости от того на сколько отклонилась мышь, меняем поворо модели
//                 thetaY += mouseOffset.X * 0.05f;
//                 thetaX += mouseOffset.Y * 0.05f;
//
//
//                 Mat4x4 viewMat = Mat4x4.GetPointAtMatrix(cameraLoc, cameraLoc + lookDir, upDir);
//                 viewMat.Invert();
//
//                 // Mat4x4 rotationMat = Mat4x4.GetRotationY(thetaY * 0.0005f) * 
//                 //                      Mat4x4.GetRotationX(thetaX * 0.0005f) * 
//                 //                      Mat4x4.GetRotationZ(thetaZ * 0.0005f);
//                 //
//                 // Mat4x4 translationMat = new Mat4x4();
//                 // translationMat.m[3, 0] = Position_model.x;
//                 // translationMat.m[3, 1] = Position_model.y;
//                 // translationMat.m[3, 2] = Position_model.z;
//                 //
//                 //
//                 // Mat4x4 mvp = projMat * viewMat * rotationMat;
//
//                 foreach (Triangle t in mesh.Triangles)
//                 {
//                     Triangle new_tri = new Triangle();
//                     Triangle proj = new Triangle();
//
//                     for (int k = 0; k < 3; k++)
//                     {
//                         new_tri.Points[k] = t.Points[k] * Mat4x4.GetRotationY(thetaY) ;
//                         new_tri.Points[k] = new_tri.Points[k] * Mat4x4.GetRotationZ(thetaZ);
//                         new_tri.Points[k] = new_tri.Points[k] * Mat4x4.GetRotationX(thetaX);
//                         new_tri.Points[k] = new_tri.Points[k] * Mat4x4.GetScaleMatrix(scale, scale, scale);
//                         new_tri.Points[k] = new_tri.Points[k] * viewMat;
//                         new_tri.Points[k] += Position_model;
//                         new_tri.Points[k] = new_tri.Points[k] * projMat;
//                     }
//
//                     VertexArray outline = new VertexArray(PrimitiveType.Lines, 6);
//                     for (int j = 0; j < 3; j++)
//                     {
//                         float x1 = (new_tri.Points[j].x + 1) * window.Size.X / 2;
//                         float y1 = (1 - (new_tri.Points[j].y + 1) * 0.5f) * window.Size.Y;
//                         float x2 = (new_tri.Points[(j + 1) % 3].x + 1) * window.Size.X / 2;
//                         float y2 = (1 - (new_tri.Points[(j + 1) % 3].y + 1) * 0.5f) * window.Size.Y;
//
//                         outline[(uint)(2 * j)] = new Vertex(new Vector2f(x1, y1), Color.White);
//                         outline[(uint)(2 * j + 1)] = new Vertex(new Vector2f(x2, y2), Color.White);
//                     }
//
//                     window.Draw(outline);
//
//
//                         // Vec4[] transformedPoints = new Vec4[3];
//                         // for (int i = 0; i < 3; i++)
//                         // {
//                         //     transformedPoints[i] = mvp * new Vec4(triangle.Points[i]);
//                         //     transformedPoints[i] /= transformedPoints[i].w;
//                         //     transformedPoints[i] += Position_model;
//                         // }
//                         //
//                         // VertexArray outline = new VertexArray(PrimitiveType.Lines, 6);
//                         // for (int j = 0; j < 3; j++)
//                         // {
//                         //     float x1 = (transformedPoints[j].x + 1) * window.Size.X / 2;
//                         //     float y1 = (1 - (transformedPoints[j].y + 1) * 0.5f) * window.Size.Y;
//                         //     float x2 = (transformedPoints[(j+1)%3].x + 1) * window.Size.X / 2;
//                         //     float y2 = (1 - (transformedPoints[(j+1)%3].y + 1) * 0.5f) * window.Size.Y;
//                         //     
//                         //     outline[(uint)(2*j)] = new Vertex(new Vector2f(x1, y1), Color.White);
//                         //     outline[(uint)(2*j+1)] = new Vertex(new Vector2f(x2, y2), Color.White);
//                         // }
//                         // window.Draw(outline);
//                     }
//
//                     window.Display();
//                 }
//             }
//         }
//     }
//
//
// // Mat4x4 transform_matrix = Mat4x4.GetRotationY(thetaY * 0.5f) * Mat4x4.GetRotationX(thetaX * 0.5f) *
// //                           Mat4x4.GetRotationZ(thetaZ * 0.5f) * projMat;
// // foreach (Triangle t in mesh.Triangles)
// // {
// //     Triangle triangle = new Triangle(t);
// //     for (int i = 0; i < 3; i++)
// //     {
// //         triangle.Points[i] *= transform_matrix;                        
// //         triangle.Points[i] += Position_model;
// //         triangle.Points[i] /= triangle.Points[i].w;
// //
// //     }
// //     VertexArray outline = new VertexArray(PrimitiveType.LineStrip, 4);
// //
// //     for (int j = 0; j < 4; j++)
// //     {
// //         float x = (triangle.Points[j % 3].x + 1) * window.Size.X / 2;
// //         float y = (triangle.Points[j % 3].y + 1) * window.Size.Y / 2;
// //         outline[(uint)j] = new Vertex(new Vector2f(x, y), Color.White);
// //     }
// //
// //     window.Draw(outline);
// //     
// // }
// // window.Display();
//
//
// // Обновление направления взгляда
// // UpdateLookDirection(ref lookDir, window, allowMouseMovement, WINDOW_WIDTH, WINDOW_HEIGHT);
// //
// // // Матрица вида
// // Mat4x4 viewMat = Mat4x4.GetPointAtMatrix(cameraLoc, cameraLoc + lookDir, upDir);
// // viewMat.Invert();
// //
// // // Очистка экрана
// // window.Clear(Color.White);
// //
// // // Рендеринг
// // List<Triangle> trianglesToDraw = ProcessTriangles(testMesh, cameraLoc, viewMat, projMat, theta);
// //
// // // Сортировка по глубине
// // trianglesToDraw = trianglesToDraw.OrderByDescending(t => 
// //     (t.Points[0].z + t.Points[1].z + t.Points[2].z) / 3.0f).ToList();
// //
// // // Отрисовка
// // RenderTriangles(window, trianglesToDraw, outlineOnly, lightDir, WINDOW_WIDTH, WINDOW_HEIGHT);
// //
// // // Обновление FPS
// // UpdateWindowTitle(window, fpsClock, testMesh.Triangles.Count, trianglesToDraw.Count);
// // fpsClock.Restart();
// //
// // window.Display();
//
//
// // static void HandleInput(RenderWindow window, ref bool[] keys, ref bool outlineOnly, 
// //     ref bool allowMouseMovement, ref bool allowRotation)
// // {
// //     window.Closed += (s, e) => window.Close();
// //
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) window.Close();
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.W)) keys[0] = true;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.A)) keys[1] = true;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.S)) keys[2] = true;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.D)) keys[3] = true;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.Space)) keys[4] = true;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.LShift)) keys[5] = true;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.Tab)) outlineOnly = !outlineOnly;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.LControl)) allowMouseMovement = !allowMouseMovement;
// //     if (Keyboard.IsKeyPressed(Keyboard.Key.R)) allowRotation = !allowRotation;
// //
// //     if (!Keyboard.IsKeyPressed(Keyboard.Key.W)) keys[0] = false;
// //     if (!Keyboard.IsKeyPressed(Keyboard.Key.A)) keys[1] = false;
// //     if (!Keyboard.IsKeyPressed(Keyboard.Key.S)) keys[2] = false;
// //     if (!Keyboard.IsKeyPressed(Keyboard.Key.D)) keys[3] = false;
// //     if (!Keyboard.IsKeyPressed(Keyboard.Key.Space)) keys[4] = false;
// //     if (!Keyboard.IsKeyPressed(Keyboard.Key.LShift)) keys[5] = false;
// // }
//
// // static void UpdateCameraPosition(ref Vec4 cameraLoc, ref Vec4 lookDir, bool[] keys)
// // {
// //     Vec4 vel = new Vec4(0, 0, 0);
// //     if (keys[0]) vel += new Vec4(0, 0, 0.1f);
// //     if (keys[1]) vel += new Vec4(-0.1f, 0, 0);
// //     if (keys[2]) vel += new Vec4(0, 0, -0.1f);
// //     if (keys[3]) vel += new Vec4(0.1f, 0, 0);
// //     if (keys[4]) vel += new Vec4(0, -0.1f, 0);
// //     if (keys[5]) vel += new Vec4(0, 0.1f, 0);
// //
// //     Vec4 tempDir = new Vec4(lookDir.x, 0, lookDir.z);
// //     tempDir.Normalize();
// //     float phi = (float)Math.Acos(Vec4.Dot(tempDir, new Vec4(0, 0, 1)));
// //     phi = tempDir.x < 0 ? phi : -phi;
// //     Vec4 rotVel = vel * Mat4x4.GetRotationYMatrix(phi);
// //     cameraLoc += rotVel;
// // }
//
// // static void UpdateLookDirection(ref Vec4 lookDir, RenderWindow window, 
// //     bool allowMouseMovement, uint windowWidth, uint windowHeight)
// // {
// //     if (allowMouseMovement)
// //     {
// //         Vector2i mousePos = Mouse.GetPosition(window);
// //         Vector2i mouseOffset = new Vector2i(
// //             mousePos.X - (int)windowWidth / 2,
// //             mousePos.Y - (int)windowHeight / 2);
// //
// //         lookDir = lookDir * Mat4x4.GetRotationYMatrix(-mouseOffset.X * 0.005f);
// //         Vec4 horDir = new Vec4(lookDir.x, 0, lookDir.z);
// //         horDir.Normalize();
// //         float phi = (float)Math.Acos(Vec4.Dot(horDir, new Vec4(0, 0, 1)));
// //         phi = horDir.x < 0 ? -phi : phi;
// //         Vec4 tempDir = lookDir * Mat4x4.GetRotationYMatrix(phi);
// //         tempDir = tempDir * Mat4x4.GetRotationXMatrix(-mouseOffset.Y * 0.005f);
// //         lookDir = tempDir * Mat4x4.GetRotationYMatrix(-phi);
// //         lookDir.Normalize();
// //
// //         Mouse.SetPosition(new Vector2i((int)windowWidth / 2, (int)windowHeight / 2), window);
// //     }
// // }
//
// // static List<Triangle> ProcessTriangles(Mesh mesh, Vec4 cameraLoc, 
// //     Mat4x4 viewMat, Mat4x4 projMat, float theta)
// // {
// //     List<Triangle> trianglesToDraw = new List<Triangle>();
// //
// //     foreach (var tri in mesh.Triangles)
// //     {
// //         Triangle newTri = new Triangle();
// //         
// //         // Применение вращения модели
// //         for (int k = 0; k < 3; k++)
// //         {
// //             newTri.Points[k] = tri.Points[k] * Mat4x4.GetRotationYMatrix(theta * 1.5f);
// //             newTri.Points[k] = newTri.Points[k] * Mat4x4.GetRotationZMatrix((float)Math.PI);
// //         }
// //
// //         // Отсечение невидимых граней
// //         newTri.Normal = Vec4.Cross(
// //             newTri.Points[2] - newTri.Points[0], 
// //             newTri.Points[1] - newTri.Points[0]);
// //         newTri.Normal.Normalize();
// //
// //         Vec4 camDir = (newTri.Points[0] + newTri.Points[1] + newTri.Points[2]) / 3 - cameraLoc;
// //         if (Vec4.Dot(newTri.Normal, camDir) >= 0)
// //             continue;
// //
// //         // Преобразование координат
// //         for (int k = 0; k < 3; k++)
// //         {
// //             newTri.Points[k] = viewMat * new Vec4(newTri.Points[k].x, newTri.Points[k].y, newTri.Points[k].z, 1);
// //         }
// //
// //         // Отсечение и проекция
// //         List<Triangle> clipped = newTri.ClipAgainstPlane(
// //             new Vec4(0, 0, 0.2f), new Vec4(0, 0, 1));
// //
// //         foreach (var t in clipped)
// //         {
// //             Triangle projected = new Triangle();
// //             for (int m = 0; m < 3; m++)
// //             {
// //                 projected.Points[m] = projMat * new Vec4(t.Points[m].x, t.Points[m].y, t.Points[m].z, 1);
// //                 projected.Points[m] /= projected.Points[m].w;
// //             }
// //             trianglesToDraw.Add(projected);
// //         }
// //     }
// //
// //     return trianglesToDraw;
// // }
//
// // static void RenderTriangles(RenderWindow window, List<Triangle> triangles, 
// //     bool outlineOnly, Vec4 lightDir, uint width, uint height)
// // {
// //     foreach (var tri in triangles)
// //     {
// //         if (outlineOnly)
// //         {
// //             VertexArray outline = new VertexArray(PrimitiveType.LineStrip, 4);
// //             
// //             for (int j = 0; j < 4; j++)
// //             {
// //                 float x = (tri.Points[j % 3].x + 1) * width / 2;
// //                 float y = (1 - (tri.Points[j % 3].y + 1) * 0.5f) * height;
// //                 outline[j] = new Vertex(new Vector2f(x, y), Color.Black);
// //             }
// //             
// //             window.Draw(outline);
// //         }
// //         else
// //         {
// //             VertexArray triangleVertices = new VertexArray(PrimitiveType.Triangles, 3);
// //             
// //             for (int j = 0; j < 3; j++)
// //             {
// //                 float x = (tri.Points[j].x + 1) * width / 2;
// //                 float y = (1 - (tri.Points[j].y + 1) * 0.5f) * height;
// //                 
// //                 float lightFactor = Math.Max(0.0f, 
// //                     Vec4.Dot(-tri.Normal, lightDir));
// //                 lightFactor = 0.3f + 0.7f * lightFactor;
// //                 
// //                 triangleVertices[j] = new Vertex(
// //                     new Vector2f(x, y),
// //                     new Color(
// //                         (byte)(153 * lightFactor),
// //                         (byte)(249 * lightFactor),
// //                         (byte)(142 * lightFactor)));
// //             }
// //             
// //             window.Draw(triangleVertices);
// //         }
// //     }
// // }
//
// // static void UpdateWindowTitle(RenderWindow window, Clock clock, 
// //     int totalTriangles, int drawnTriangles)
// // {
// //     float fps = 1.0f / clock.ElapsedTime.AsSeconds();
// //     window.SetTitle(
// //         $"FPS: {fps:0}; Triangles rendering: {totalTriangles}; " +
// //         $"Triangles drawing: {drawnTriangles}");
// // }