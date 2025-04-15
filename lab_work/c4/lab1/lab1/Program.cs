using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace Custom3DEngine
{
    public class Renderer
    {
        private const int Width = 1920;
        private const int Height = 1080;

        private RenderWindow? _window;  
        private Mesh3D? _mesh;         
        private Matrix4x4? _projection;  
        
        private Vector4D _cameraPos = new Vector4D(0, -2f, -5);

        public void Run()
        {
            Initialize();
            MainLoop();
        }

        private void Initialize()
        {
            _window = new RenderWindow(new VideoMode((uint)Width, (uint)Height), "3D Renderer");
            _window.SetVerticalSyncEnabled(true);
            _window.SetMouseCursorVisible(false);
            _window.Closed += (s, e) => _window.Close();

            _mesh = new Mesh3D();
            if (!_mesh.LoadFromFile("2.obj"))
                _mesh.CreateCube();

            _projection = Matrix4x4.Projection(Width, Height);
        }

        private void MainLoop()
        {
            float rotX = 0, rotY = 0, rotZ = 0, scale = 1;
            Vector4D modelPos = new Vector4D();
            

            while (_window!.IsOpen)
            {
                _window.DispatchEvents();
                _window.Clear(Color.Black);

                ProcessInput(ref modelPos, ref rotX, ref rotY, ref rotZ, ref scale);
                RenderScene(rotX, rotY, rotZ, modelPos, scale);

                _window.Display();
            }
        }

        private void ProcessInput(ref Vector4D pos, ref float rotX, ref float rotY, ref float rotZ, ref float scale)
        {                
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) _window.Close();
            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) pos.Z += 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) pos.Z -= 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) pos.X -= 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) pos.X += 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space)) pos.Y += 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift)) pos.Y -= 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q)) rotZ += 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.E)) rotZ -= 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Z)) scale += 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.X) && scale>0.1f) scale -= 0.1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.R)) { pos = new Vector4D(); rotX = rotY = rotZ = 0; scale = 1; }

            var mousePos = Mouse.GetPosition(_window!);
            rotY += (mousePos.X - Width/2) * 0.05f;
            rotX += (mousePos.Y - Height/2) * 0.05f;
            Mouse.SetPosition(new SFML.System.Vector2i(Width/2, Height/2), _window!);
        }

        private void RenderScene(float rotX, float rotY, float rotZ, Vector4D modelPos, float scale)
        {
            var view = Matrix4x4.LookAt(
                _cameraPos,
                _cameraPos + new Vector4D(0, 0, 1),
                new Vector4D(0, 1, 0));
            view.Invert();

            foreach (var tri in _mesh!.Triangles)
            {
                var transformed = new Triangle3D(
                    ApplyTransforms(tri.Vertices[0], rotX, rotY, rotZ, scale, modelPos, view),
                    ApplyTransforms(tri.Vertices[1], rotX, rotY, rotZ,scale, modelPos, view),
                    ApplyTransforms(tri.Vertices[2], rotX, rotY, rotZ, scale, modelPos, view));

                DrawTriangle(transformed);
            }
        }

        private Vector4D ApplyTransforms(Vector4D v, float rx, float ry, float rz, float scale, Vector4D pos, Matrix4x4 view)
        {
            Vector4D temp =  (v * Matrix4x4.RotationX(rx));
            temp =  (temp * Matrix4x4.RotationY(ry));
            temp =  (temp * Matrix4x4.RotationZ(rz));            
            temp = view * (temp * Matrix4x4.Scale(scale, scale, scale));
            temp += pos;
            return _projection! * temp;
        }

        private void DrawTriangle(Triangle3D tri)
        {
            var va = new VertexArray(PrimitiveType.Lines, 6);
            for (int i = 0; i < 3; i++)
            {
                Vector4D v1 = tri.Vertices[i];
                Vector4D v2 = tri.Vertices[(i + 1) % 3];

                float x1 = (v1.X + 1) * Width / 2;
                float y1 = (1 - (v1.Y + 1) * 0.5f) * Height;
                float x2 = (v2.X + 1) * Width / 2;
                float y2 = (1 - (v2.Y + 1) * 0.5f) * Height;

                va[(uint)(2 * i)] = new Vertex(new SFML.System.Vector2f(x1, y1), Color.White);
                va[(uint)(2 * i + 1)] = new Vertex(new SFML.System.Vector2f(x2, y2), Color.White);
            }
            _window!.Draw(va);
        }

        public static void Main() => new Renderer().Run();
    }
}