using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Windowing.GraphicsLibraryFramework;


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
        _mesh = new Mesh();
        // _mesh.DefineAsCube();
        _mesh.LoadFromFile("1.obj");
        

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
}

public static class Program
{
    public static void Main()
    {
        
        var nativeSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Mesh Viewer (OpenTK)"
        };


        using var window = new MeshWindow(GameWindowSettings.Default, nativeSettings);
        window.Run();
    }
}
