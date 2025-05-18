using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

class MyGame : GameWindow
{
    public MyGame()
        : base(1920, 1080, GraphicsMode.Default, "OpenGL на Linux — OpenTK 3.3.3")
    {
        VSync = VSyncMode.On;
    }

    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);
        GL.ClearColor(Color4.CornflowerBlue);
    }

    protected override void OnResize(System.EventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Width, Height);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, Width, Height, 0, -1, 1); // 2D координаты
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        if (Keyboard.GetState().IsKeyDown(Key.Escape))
            Exit();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        // Рисуем красную линию
        const int WINDOW_WIDTH = 1920;
        const int WINDOW_HEIGHT = 1080;

        // Инициализация камеры
        Vec4 cameraLoc = new Vec4(0, -2.0f, -5);
        Vec4 lookDir = new Vec4(0, 0, 1);
        Vec4 upDir = new Vec4(0, 1, 0);


        // Создание окна
        window.SetMouseCursorVisible(false);
        Mouse.SetPosition(new Vector2i((int)WINDOW_WIDTH / 2, (int)WINDOW_HEIGHT / 2), window);

        // Загрузка модели
        Mesh mesh = new Mesh();
        if (!mesh.LoadFromFile("1.obj"))
        {
            return;
        }

        // mesh.DefineAsCube();


        // параметры модели
        float thetaY = 0;
        float thetaX = 0;
        float thetaZ = 0;
        float scale = 1f;

        Vec4 Position_model = new Vec4();


        // Матрица проекции
        Mat4x4 projMat = Mat4x4.GetProjectionMatrix(1920, 1080, 0.1f, 1000f, 90f);


        // Главный цикл
        
        
        base.OnUpdateFrame(e);

        var input = Keyboard.GetState();

        if (input.IsKeyDown(Key.Escape))
        {
            Exit();
        }

        // Перемещение модели
        if (input.IsKeyDown(Key.W)) Position_model.Z += 0.1f;
        if (input.IsKeyDown(Key.S)) Position_model.Z -= 0.1f;
        if (input.IsKeyDown(Key.D)) Position_model.X += 0.1f;
        if (input.IsKeyDown(Key.A)) Position_model.X -= 0.1f;
        if (input.IsKeyDown(Key.Space)) Position_model.Y += 0.1f;
        if (input.IsKeyDown(Key.LShift)) Position_model.Y -= 0.1f;

        // Вращение модели по оси Z
        if (input.IsKeyDown(Key.Q)) thetaZ += 0.1f;
        if (input.IsKeyDown(Key.E)) thetaZ -= 0.1f;

        // Масштаб
        if (input.IsKeyDown(Key.Z) && scale > 0.1f) scale -= 0.01f;
        if (input.IsKeyDown(Key.X)) scale += 0.01f;

        // Сброс позиции и углов
        if (input.IsKeyDown(Key.R))
        {
            Position_model = new Vec4();
            thetaX = 0f;
            thetaY = 0f;
            thetaZ = 0f;
        }

        //обработка двидение мыши
        // Vector2i mouseOffset = new Vector2i(0, 0);
        // Vector2i mousePos = Mouse.GetPosition(window);
        // mouseOffset.X = mousePos.X - WINDOW_WIDTH / 2;
        // mouseOffset.Y = mousePos.Y - WINDOW_HEIGHT / 2;
        // Mouse.SetPosition(new Vector2i(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2), window);

        // в зависимости от того на сколько отклонилась мышь, меняем поворо модели
        // thetaY += mouseOffset.X * 0.05f;
        // thetaX += mouseOffset.Y * 0.05f;


        Mat4x4 viewMat = Mat4x4.GetPointAtMatrix(cameraLoc, cameraLoc + lookDir, upDir);
        viewMat.Invert();

        // Mat4x4 rotationMat = Mat4x4.GetRotationY(thetaY * 0.0005f) * 
        //                      Mat4x4.GetRotationX(thetaX * 0.0005f) * 
        //                      Mat4x4.GetRotationZ(thetaZ * 0.0005f);
        //
        // Mat4x4 translationMat = new Mat4x4();
        // translationMat.m[3, 0] = Position_model.x;
        // translationMat.m[3, 1] = Position_model.y;
        // translationMat.m[3, 2] = Position_model.z;
        //
        //
        // Mat4x4 mvp = projMat * viewMat * rotationMat;

        foreach (Triangle t in mesh.Triangles)
        {
            Triangle new_tri = new Triangle();
            Triangle proj = new Triangle();

            for (int k = 0; k < 3; k++)
            {
                new_tri.Points[k] = t.Points[k] * Mat4x4.GetRotationY(thetaY);
                new_tri.Points[k] = new_tri.Points[k] * Mat4x4.GetRotationZ(thetaZ);
                new_tri.Points[k] = new_tri.Points[k] * Mat4x4.GetRotationX(thetaX);
                new_tri.Points[k] = new_tri.Points[k] * Mat4x4.GetScaleMatrix(scale, scale, scale);
                new_tri.Points[k] = new_tri.Points[k] * viewMat;
                new_tri.Points[k] += Position_model;
                new_tri.Points[k] = new_tri.Points[k] * projMat;
            }

            // VertexArray outline = new VertexArray(PrimitiveType.Lines, 6);
            for (int j = 0; j < 3; j++)
            {
                float x1 = (new_tri.Points[j].x + 1) * WINDOW_HEIGHT / 2;
                float y1 = (1 - (new_tri.Points[j].y + 1) * 0.5f) * WINDOW_WIDTH;
                float x2 = (new_tri.Points[(j + 1) % 3].x + 1) * WINDOW_HEIGHT / 2;
                float y2 = (1 - (new_tri.Points[(j + 1) % 3].y + 1) * 0.5f) * WINDOW_WIDTH;

                outline[(uint)(2 * j)] = new Vertex(new Vector2f(x1, y1), Color.White);
                outline[(uint)(2 * j + 1)] = new Vertex(new Vector2f(x2, y2), Color.White);
            }

            window.Draw(outline);
        }

        window.Display();
    }


    GL.Color3(1.0f, 0.0f, 0.0f);
    GL.Begin(PrimitiveType.Lines);
    GL.Vertex2(100, 100);
    GL.Vertex2(700, 500);
    GL.End();

    SwapBuffers();
}

static void Main()
{
    using (var game = new MyGame())
    {
        game.Run(60.0);
    }
}


void DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
{
    GL.Begin(PrimitiveType.Triangles);

    GL.Color3(1.0f, 0.0f, 0.0f); // Красный цвет (можно менять)
    GL.Vertex2(v1);

    GL.Color3(0.0f, 1.0f, 0.0f); // Зелёный
    GL.Vertex2(v2);

    GL.Color3(0.0f, 0.0f, 1.0f); // Синий
    GL.Vertex2(v3);

    GL.End();
}

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
//                     }
//
//                     window.Display();
//                 }
//             }
//         }
//     }