using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;

namespace Wireframe3DViewer
{
    class Program : GameWindow
    {
        Vec4 cameraLoc = new Vec4(0, -2.0f, -5);
        Vec4 lookDir = new Vec4(0, 0, 1);
        Vec4 upDir = new Vec4(0, 1, 0);
        Mesh mesh = new Mesh();
        Vec4 Position_model = new Vec4();
        float thetaY = 0, thetaX = 0, thetaZ = 0, scale = 1f;
        Mat4x4 projMat;

        public Program() : base(
            1920, 1080,
            GraphicsMode.Default,
            "3D Engine (OpenGL)",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            2, 1, // OpenGL 2.1
            GraphicsContextFlags.Default)
        {
            CursorVisible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);

            if (!mesh.LoadFromFile("/home/ivannz/Programing/Computer_Grafic_NSTU/lab_work/c4/RGZ0/RGZ0/1.obj"))
            {
                Console.WriteLine("Failed to load model");
                Exit();
            }

            projMat = Mat4x4.GetProjectionMatrix(Width, Height, 0.1f, 1000f, 90f);
            Mouse.SetPosition(X + Width / 2, Y + Height / 2);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState key = Keyboard.GetState();
            if (key.IsKeyDown(Key.Escape))
                Exit();

            float moveSpeed = 0.1f;

            if (key.IsKeyDown(Key.W)) Position_model.z += moveSpeed;
            if (key.IsKeyDown(Key.S)) Position_model.z -= moveSpeed;
            if (key.IsKeyDown(Key.D)) Position_model.x += moveSpeed;
            if (key.IsKeyDown(Key.A)) Position_model.x -= moveSpeed;
            if (key.IsKeyDown(Key.Space)) Position_model.y += moveSpeed;
            if (key.IsKeyDown(Key.LShift)) Position_model.y -= moveSpeed;

            if (key.IsKeyDown(Key.Q)) thetaZ += 0.1f;
            if (key.IsKeyDown(Key.E)) thetaZ -= 0.1f;
            if (key.IsKeyDown(Key.Z)) scale = Math.Max(0.1f, scale - 0.01f);
            if (key.IsKeyDown(Key.X)) scale += 0.01f;
            if (key.IsKeyDown(Key.R))
            {
                Position_model = new Vec4();
                thetaX = thetaY = thetaZ = 0;
            }

            // Обработка мыши
            var mouse = Mouse.GetState();
            int dx = mouse.X - (X + Width / 2);
            int dy = mouse.Y - (Y + Height / 2);
            thetaY += dx * 0.05f;
            thetaX += dy * 0.05f;
            Mouse.SetPosition(X + Width / 2, Y + Height / 2);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            Mat4x4 viewMat = Mat4x4.GetPointAtMatrix(cameraLoc, cameraLoc + lookDir, upDir);
            viewMat.Invert();

            foreach (Triangle t in mesh.Triangles)
            {
                Triangle tri = new Triangle();
                for (int i = 0; i < 3; i++)
                {
                    Vec4 v = t.Points[i];
                    v = v * Mat4x4.GetRotationY(thetaY);
                    v = v * Mat4x4.GetRotationZ(thetaZ);
                    v = v * Mat4x4.GetRotationX(thetaX);
                    v = v * Mat4x4.GetScaleMatrix(scale, scale, scale);
                    v += Position_model;
                    v = v * viewMat;
                    v = v * projMat;
                    tri.Points[i] = v;
                }

                DrawWireframeTriangle(tri);
            }

            SwapBuffers();
        }

        void DrawWireframeTriangle(Triangle tri)
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1.0f, 1.0f, 1.0f);
            for (int i = 0; i < 3; i++)
            {
                var p1 = tri.Points[i];
                var p2 = tri.Points[(i + 1) % 3];

                float x1 = (p1.x + 1) * Width / 2f;
                float y1 = (1 - (p1.y + 1) * 0.5f) * Height;
                float x2 = (p2.x + 1) * Width / 2f;
                float y2 = (1 - (p2.y + 1) * 0.5f) * Height;

                GL.Vertex2(x1, y1);
                GL.Vertex2(x2, y2);
            }
            GL.End();
        }

        [STAThread]
        static void Main()
        {
            using (var window = new Program())
                window.Run(60.0);
        }
    }
}
