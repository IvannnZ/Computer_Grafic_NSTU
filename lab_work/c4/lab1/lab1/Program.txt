using System;
using System.Numerics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

class WireframeEditor
{
    private const int Width = 800;
    private const int Height = 600;

    private RenderWindow window;
    private readonly Mesh cube;
    private Vector3 position = new Vector3(0, 0, -3);
    private Vector3 rotation = Vector3.Zero;
    private float scale = 1.0f;
    private Vector2i? lastMousePos;
    private bool isRotating = false;
    private readonly Dictionary<Keyboard.Key, bool> keys = new();

    public WireframeEditor()
    {
        window = new RenderWindow(new VideoMode(Width, Height), "3D Wireframe Editor");
        window.SetVerticalSyncEnabled(true);

        cube = CreateCube();

        window.Closed += (s, e) => window.Close();
        window.KeyPressed += (s, e) => keys[e.Code] = true;
        window.KeyReleased += (s, e) => keys[e.Code] = false;
        window.MouseButtonPressed += HandleMousePress;
        window.MouseButtonReleased += HandleMouseRelease;
        window.MouseMoved += HandleMouseMove;
        window.MouseWheelScrolled += HandleMouseWheel;
    }

    public void Run()
    {
        Clock clock = new Clock();

        while (window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();

            window.DispatchEvents();
            HandleInput(deltaTime);

            window.Clear(Color.Black);
            DrawWireframe();
            window.Display();
        }
    }

    private Mesh CreateCube()
    {
        var vertices = new Vector3[]
        {
            new(-0.5f, -0.5f, -0.5f), new(0.5f, -0.5f, -0.5f),
            new(0.5f, 0.5f, -0.5f), new(-0.5f, 0.5f, -0.5f),
            new(-0.5f, -0.5f, 0.5f), new(0.5f, -0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f), new(-0.5f, 0.5f, 0.5f)
        };

        var edges = new (int, int)[]
        {
            (0, 1), (1, 2), (2, 3), (3, 0), // задняя грань
            (4, 5), (5, 6), (6, 7), (7, 4), // передняя грань
            (0, 4), (1, 5), (2, 6), (3, 7) // соединения
        };
ы
        return new Mesh { Vertices = vertices, Edges = edges };
    }

    private void DrawWireframe()
    {
        var transform = Matrix4x4.CreateScale(scale) *
                        Matrix4x4.CreateRotationX(rotation.X) *
                        Matrix4x4.CreateRotationY(rotation.Y) *
                        Matrix4x4.CreateTranslation(position);

        foreach (var (i1, i2) in cube.Edges)
        {
            var v1 = ProjectPoint(Vector3.Transform(cube.Vertices[i1], transform));
            var v2 = ProjectPoint(Vector3.Transform(cube.Vertices[i2], transform));

            var line = new VertexArray(PrimitiveType.Lines, 2);
            line[0] = new Vertex(v1, Color.White);
            line[1] = new Vertex(v2, Color.White);
            window.Draw(line);
        }
    }

    private Vector2f ProjectPoint(Vector3 point)
    {
        // Простая перспективная проекция
        float fov = Width / 2f;
        float z = point.Z + 5; // Добавляем смещение для видимости
        return new Vector2f(
            point.X * fov / z + Width / 2,
            -point.Y * fov / z + Height / 2);
    }

    private void HandleInput(float deltaTime)
    {
        const float moveSpeed = 2.0f;
        const float rotateSpeed = 0.2f;

        // Перемещение
        if (keys.TryGetValue(Keyboard.Key.W, out bool w) && w) position.Z += moveSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.S, out bool s) && s) position.Z -= moveSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.A, out bool a) && a) position.X -= moveSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.D, out bool d) && d) position.X += moveSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.Q, out bool q) && q) position.Y += moveSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.E, out bool e) && e) position.Y -= moveSpeed * deltaTime;

        // Стрелки для поворота (альтернатива мышке)
        if (keys.TryGetValue(Keyboard.Key.Left, out bool left) && left) rotation.Y -= rotateSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.Right, out bool right) && right) rotation.Y += rotateSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.Up, out bool up) && up) rotation.X -= rotateSpeed * deltaTime;
        if (keys.TryGetValue(Keyboard.Key.Down, out bool down) && down) rotation.X += rotateSpeed * deltaTime;
    }

    private void HandleMousePress(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            isRotating = true;
            lastMousePos = new Vector2i(e.X, e.Y);
        }
    }

    private void HandleMouseRelease(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            isRotating = false;
            lastMousePos = null;
        }
    }

    private void HandleMouseMove(object sender, MouseMoveEventArgs e)
    {
        if (!isRotating || lastMousePos == null) return;

        var delta = new Vector2i(e.X - lastMousePos.Value.X, e.Y - lastMousePos.Value.Y);
        rotation.Y += delta.X * 0.005f;
        rotation.X += delta.Y * 0.005f;

        lastMousePos = new Vector2i(e.X, e.Y);
    }

    private void HandleMouseWheel(object sender, MouseWheelScrollEventArgs e)
    {
        scale += e.Delta * 0.1f;
        scale = Math.Clamp(scale, 0.1f, 5f);
    }
}

class Mesh
{
    public Vector3[] Vertices { get; set; }
    public (int, int)[] Edges { get; set; }
}

class Program
{
    static void Main()
    {
        new WireframeEditor().Run();
    }
}