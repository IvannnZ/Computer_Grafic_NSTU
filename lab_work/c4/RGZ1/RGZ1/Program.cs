using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class MeshWindow : GameWindow
{
    private int _vao, _vbo;
    private int _shaderProgram;
    private float[] _vertexData;
    private int _vertexCount;

    private Mesh _mesh;

    public MeshWindow(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
        : base(gameSettings, nativeSettings) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Пример: куб
        _mesh = new Mesh();
        _mesh.DefineAsCube();

        _vertexData =_mesh.GetVertexArray();
        _vertexCount = _vertexData.Length / 3;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertexData.Length * sizeof(float), _vertexData, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        _shaderProgram = CreateBasicShader();
        GL.UseProgram(_shaderProgram);

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(_shaderProgram);
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
}