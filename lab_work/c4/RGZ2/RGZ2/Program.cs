using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Concurrent;

using DelaunatorSharp;
using System.Collections.Generic;

public class MeshWindow : GameWindow
{
    private int _vao, _vbo;
    private int _shaderProgram;
    // private float[] _vertexData;
    private int _vertexCount;
    // private Mesh _mesh;
    private float[] _vertexData = Array.Empty<float>();
    private Mesh _mesh = new Mesh();
    public MeshWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
        : base(gameSettings, nativeSettings) { }
    
    private Vector3 _rotation = Vector3.Zero;
    private Vector3 _position = Vector3.Zero;


    protected override void OnLoad()
    {
        base.OnLoad();

        // Пример: куб
        // _mesh = new Mesh();
        // _mesh.DefineAsCube();
        // _mesh.LoadFromFile("3.obj");
        

        _vertexData = _mesh.GetVertexArrayWithNormals();
        _vertexCount = _vertexData.Length / 6;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertexData.Length * sizeof(float), _vertexData, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        
        _shaderProgram = CreateBasicShader();
        GL.UseProgram(_shaderProgram);

        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Enable(EnableCap.DepthTest); // Включаем Z-buffer
    }
    
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        while (Program.viewerQueue.TryDequeue(out Mesh newMesh))
        {
            SetMesh(newMesh);
        }

        // Вращение и движение камеры (как у тебя уже есть)
        const float rotSpeed = 1.5f;
        const float moveSpeed = 1.5f;

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Left)) _rotation.Y += rotSpeed * (float)args.Time;
        if (input.IsKeyDown(Keys.Right)) _rotation.Y -= rotSpeed * (float)args.Time;
        if (input.IsKeyDown(Keys.Up)) _rotation.X += rotSpeed * (float)args.Time;
        if (input.IsKeyDown(Keys.Down)) _rotation.X -= rotSpeed * (float)args.Time;

        if (input.IsKeyDown(Keys.W)) _position.Z -= moveSpeed * (float)args.Time;
        if (input.IsKeyDown(Keys.S)) _position.Z += moveSpeed * (float)args.Time;
        if (input.IsKeyDown(Keys.A)) _position.X -= moveSpeed * (float)args.Time;
        if (input.IsKeyDown(Keys.D)) _position.X += moveSpeed * (float)args.Time;
    }



    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderProgram);
        
        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateRotationX(_rotation.X);
        model *= Matrix4.CreateRotationY(_rotation.Y);
        model *= Matrix4.CreateTranslation(_position);

        
        Matrix4 view = Matrix4.LookAt(new Vector3(2, 2, 2), Vector3.Zero, Vector3.UnitY);
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);

        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref model);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "view"), false, ref view);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref projection);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        GL.DeleteProgram(_shaderProgram);
    }

    private float[] GetVertexArray(Mesh mesh)
    {
        List<float> vertices = new List<float>();
        foreach (var tri in mesh.Triangles)
        {
            foreach (var v in tri.Points)
            {
                vertices.Add(v.x);
                vertices.Add(v.y);
                vertices.Add(v.z);
            }
        }
        return vertices.ToArray();
    }

    private int CreateBasicShader()
    {
        string vertexShaderSource = File.ReadAllText("/home/ivannz/Programing/Computer_Grafic_NSTU/lab_work/c4/RGZ1/RGZ1/vertex.glsl");
        string fragmentShaderSource = File.ReadAllText("/home/ivannz/Programing/Computer_Grafic_NSTU/lab_work/c4/RGZ1/RGZ1/fragment.glsl");
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success1);
        if (success1 != (int)All.True)
            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success2);
        if (success2 != (int)All.True)
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success3);
        if (success3 != (int)All.True)
            Console.WriteLine(GL.GetProgramInfoLog(program));

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return program;
    }
    
    public void SetMesh(Mesh mesh)
    {
        _mesh = mesh;
        _vertexData = _mesh.GetVertexArrayWithNormals();
        _vertexCount = _vertexData.Length / 6;

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertexData.Length * sizeof(float), _vertexData, BufferUsageHint.StaticDraw);
    }

}


public class PointEditorWindow : GameWindow
{
    private List<Vector3> _points = new();
    private int _selectedPointIndex = -1;

    private int _vao, _vbo;
    private int _shaderProgram;
    private bool _needsMeshUpdate = true;
    private Mesh _generatedMesh = new();

    public Action<Mesh>? OnMeshGenerated;

    public PointEditorWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
        : base(gameSettings, nativeSettings) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        _shaderProgram = CreateBasicShader();

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.ClearColor(0.2f, 0.2f, 0.3f, 1.0f);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        var mouse = MouseState.Position;
        Vector2 world = new Vector2(mouse.X / Size.X * 2f - 1f, 1f - mouse.Y / Size.Y * 2f);
        Vector3 clickPoint = new Vector3(world.X, world.Y, 0);

        if (e.Button == MouseButton.Left)
        {
            _points.Add(clickPoint);
            _needsMeshUpdate = true;
        }
        else if (e.Button == MouseButton.Right)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                if ((clickPoint - _points[i]).Length < 0.05f)
                {
                    _selectedPointIndex = i;
                    break;
                }
            }
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (_selectedPointIndex >= 0 && _selectedPointIndex < _points.Count)
        {
            var point = _points[_selectedPointIndex];
            point.Z += e.OffsetY * 0.1f;
            _points[_selectedPointIndex] = point;
            _needsMeshUpdate = true;
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (_needsMeshUpdate)
        {
            UpdateMesh();
            _needsMeshUpdate = false;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(_shaderProgram);

        GL.PointSize(10f);  // <-- Увеличенный размер точек

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _points.Count * 3 * sizeof(float), _points.ToArray(), BufferUsageHint.DynamicDraw);

        GL.DrawArrays(PrimitiveType.Points, 0, _points.Count);

        SwapBuffers();
    }

    public static Mesh Triangulate(List<Vector3> points)
    {
        Mesh mesh = new Mesh();
        if (points == null || points.Count < 3)
            return mesh;

        IPoint[] delaunayPoints = new IPoint[points.Count];
        for (int i = 0; i < points.Count; i++)
            delaunayPoints[i] = new Point(points[i].X, points[i].Y);

        var delaunator = new Delaunator(delaunayPoints);

        for (int i = 0; i < delaunator.Triangles.Length; i += 3)
        {
            int i0 = delaunator.Triangles[i];
            int i1 = delaunator.Triangles[i + 1];
            int i2 = delaunator.Triangles[i + 2];

            var p0 = points[i0];
            var p1 = points[i1];
            var p2 = points[i2];

            mesh.Triangles.Add(new Triangle(
                new Vec4(p0.X, p0.Y, p0.Z),
                new Vec4(p1.X, p1.Y, p1.Z),
                new Vec4(p2.X, p2.Y, p2.Z)));
        }

        return mesh;
    }

  
    
    

    private void UpdateMesh()
    {
        if (_points.Count < 3)
            return;
        for (int i = 0; i < _points.Count; i++){
            Console.WriteLine(_points[i]);
        }
        Console.WriteLine("\n");
        _generatedMesh = Triangulate(_points);
        Console.WriteLine(_generatedMesh.Triangles.Count);
        OnMeshGenerated?.Invoke(_generatedMesh);
    }


    // private void UpdateMesh()
    // {
    //     if (_points.Count < 3)
    //         return;
    //
    //     _generatedMesh = new Mesh();
    //
    //     
    //     // Простейшая триангуляция (по порядку) — ты можешь заменить на свою
    //     for (int i = 0; i + 2 < _points.Count; i += 3)
    //     {
    //         Vec4 v1 = new Vec4(_points[i].X,     _points[i].Y,     _points[i].Z);
    //         Vec4 v2 = new Vec4(_points[i + 1].X, _points[i + 1].Y, _points[i + 1].Z);
    //         Vec4 v3 = new Vec4(_points[i + 2].X, _points[i + 2].Y, _points[i + 2].Z);
    //
    //         Triangle tri = new Triangle(v1, v2, v3);
    //         _generatedMesh.Triangles.Add(tri);
    //     }
    //
    //
    //     // Отдаём полученный Mesh обратно
    //     OnMeshGenerated?.Invoke(_generatedMesh);
    // }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        GL.DeleteProgram(_shaderProgram);
    }

    private int CreateBasicShader()
    {
        string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
                gl_PointSize = 10.0;
            }";

        string fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(1, 0, 0, 1);
            }";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        return program;
    }
    
    
}



public static class Program
{
    public static ConcurrentQueue<Mesh> viewerQueue = new ConcurrentQueue<Mesh>();

    
    public static void Main()
    {
        var inputSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(600, 600),
            Title = "Input Window - Draw 2D Polygon"
        };

        var meshWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Mesh Viewer"
        };

        MeshWindow? meshWin = null;

        var inputWin = new PointEditorWindow(GameWindowSettings.Default, inputSettings);
        meshWin = new MeshWindow(GameWindowSettings.Default, meshWindowSettings);

        // Когда в окне рисования создан меш — кладём его в очередь
        inputWin.OnMeshGenerated = (Mesh mesh) =>
        {
            viewerQueue.Enqueue(mesh);
        };

        // Запускаем окно просмотра в отдельном потоке
        var meshThread = new Thread(() =>
        {
            meshWin.Run();
        });

        meshThread.Start();

        // В главном потоке запускаем окно редактирования
        inputWin.Run();
    }



    // public static void Main()
    // {
    //     var native = new NativeWindowSettings() { ClientSize = new Vector2i(800, 600), Title = "Point Editor" };
    //     var editor = new PointEditorWindow(GameWindowSettings.Default, native);
    //
    //     var viewerSettings = new NativeWindowSettings() { ClientSize = new Vector2i(800, 600), Title = "3D Viewer" };
    //     var viewer = new MeshWindow(GameWindowSettings.Default, viewerSettings);
    //
    //     editor.OnMeshGenerated = mesh =>
    //     {
    //         viewer.SetMesh(mesh);
    //     };
    //
    //     editor.Run();
    // }
}
