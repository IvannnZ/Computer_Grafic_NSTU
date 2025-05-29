// using OpenTK.Windowing.Common;
// using OpenTK.Windowing.Desktop;
// using OpenTK.Mathematics;
// using OpenTK.Graphics.OpenGL4;
//
// using OpenTK.Windowing.GraphicsLibraryFramework;
//
// public class MainWindow : GameWindow
// {
//     private int _vao;
//     private int _vbo;
//     private int _ebo;
//     private int _shaderProgram;
//
//     // Пример вершин (можно заменить на данные из .obj)
//     private readonly float[] _vertices = {
//         // позиция           // цвет
//          0.0f,  0.5f, 0.0f,  1f, 0f, 0f,
//         -0.5f, -0.5f, 0.0f,  0f, 1f, 0f,
//          0.5f, -0.5f, 0.0f,  0f, 0f, 1f,
//     };
//
//     private readonly int[] _indices = {
//         0, 1, 2
//     };
//
//     public MainWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
//     {
//         
//         
//     }
//
//     protected override void OnLoad()
//     {
//         base.OnLoad();
//
//         GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
//
//         _vao = GL.GenVertexArray();
//         _vbo = GL.GenBuffer();
//         _ebo = GL.GenBuffer();
//
//         GL.BindVertexArray(_vao);
//
//         GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
//         GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
//
//         GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
//         GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices, BufferUsageHint.StaticDraw);
//
//         // Позиции
//         GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
//         GL.EnableVertexAttribArray(0);
//         // Цвета
//         GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
//         GL.EnableVertexAttribArray(1);
//
//         // Шейдеры
//         var vertexShaderSource = @"
//             #version 330 core
//             layout(location = 0) in vec3 aPosition;
//             layout(location = 1) in vec3 aColor;
//             out vec3 vertexColor;
//             void main()
//             {
//                 gl_Position = vec4(aPosition, 1.0);
//                 vertexColor = aColor;
//             }
//         ";
//
//         var fragmentShaderSource = @"
//             #version 330 core
//             in vec3 vertexColor;
//             out vec4 FragColor;
//             void main()
//             {
//                 FragColor = vec4(vertexColor, 1.0);
//             }
//         ";
//
//         var vertexShader = GL.CreateShader(ShaderType.VertexShader);
//         GL.ShaderSource(vertexShader, vertexShaderSource);
//         GL.CompileShader(vertexShader);
//
//         var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
//         GL.ShaderSource(fragmentShader, fragmentShaderSource);
//         GL.CompileShader(fragmentShader);
//
//         _shaderProgram = GL.CreateProgram();
//         GL.AttachShader(_shaderProgram, vertexShader);
//         GL.AttachShader(_shaderProgram, fragmentShader);
//         GL.LinkProgram(_shaderProgram);
//
//         GL.DeleteShader(vertexShader);
//         GL.DeleteShader(fragmentShader);
//
//         GL.Enable(EnableCap.DepthTest);
//     }
//
//     protected override void OnRenderFrame(FrameEventArgs args)
//     {
//         base.OnRenderFrame(args);
//
//         GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
//         GL.UseProgram(_shaderProgram);
//         GL.BindVertexArray(_vao);
//         GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
//
//         SwapBuffers();
//     }
//
//     protected override void OnUnload()
//     {
//         base.OnUnload();
//         GL.DeleteBuffer(_vbo);
//         GL.DeleteBuffer(_ebo);
//         GL.DeleteVertexArray(_vao);
//         GL.DeleteProgram(_shaderProgram);
//     }
// }
//
// class Program
// {
//     static void Main(string[] args)
//     {
//         
//         
//         
//         var gws = GameWindowSettings.Default;
//         var nws = new NativeWindowSettings()
//         {
//             Title = "OpenGL + C# (OpenTK)",
//             Size = new Vector2i(800, 600)
//         };
//
//         using var window = new MainWindow(gws, nws);
//         window.Run();
//     }
// }


using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4; // Для GL.*
using OpenTK.Mathematics; // Для Vector3, Matrix4 и т.п.


// Определяем класс приложения
class Game : GameWindow
{
    public Game(GameWindowSettings gwSettings, NativeWindowSettings nwSettings)
        : base(gwSettings, nwSettings)
    {
    }

    private int _program;
    private ObjModel _model;
    private int _vao, _vbo, _nbo, _ebo;

    protected override void OnLoad()
    {
        // Пример строки шейдера (или загрузка из файла)
        string vertexSource = @"
        #version 330 core
        layout(location = 0) in vec3 aPos;
        layout(location = 1) in vec3 aNormal;
        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;
        out vec3 Normal;
        void main()
        {
            gl_Position = projection * view * model * vec4(aPos, 1.0);
            Normal = aNormal;
        }";
        string fragmentSource = @"
        #version 330 core
        in vec3 Normal;
        out vec4 FragColor;
        void main()
        {
            // Простое затенение по нормали (для наглядности)
            vec3 color = normalize(Normal) * 0.5 + 0.5;
            FragColor = vec4(color, 1.0);
        }";

        // Создаём и компилируем вершинный шейдер
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);
        GL.CompileShader(vertexShader);
        // (Здесь хорошо бы проверить статус компиляции через GL.GetShader(...))

        // Фрагментный шейдер
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        GL.CompileShader(fragmentShader);

        // Линкуем программу
        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        // После успешной линковки можно удалить шейдеры:
        GL.DetachShader(shaderProgram, vertexShader);
        GL.DetachShader(shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);


        // Сохраним ID программы для использования при отрисовке
        _program = shaderProgram;

        // По умолчанию включаем тест глубины, чтобы правильно рисовать 3D
        GL.Enable(EnableCap.DepthTest);


        _model = ObjModel.LoadFromFile("2.obj");

// Генерация буферов
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _nbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

// Вершины
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _model.Vertices.Length * sizeof(float), _model.Vertices,
            BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

// Нормали
        if (_model.Normals != null)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _nbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _model.Normals.Length * sizeof(float), _model.Normals,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
        }

// Индексы
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _model.Indices.Length * sizeof(int), _model.Indices,
            BufferUsageHint.StaticDraw);

        GL.BindVertexArray(0);
    }


// Объявляем поля для углов/масштаба
    float angleX = 0, angleY = 0, scale = 1.0f;
    Vector3 position = Vector3.Zero;

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        // Обработка ввода (пример для OpenTK 4):
        var input = KeyboardState;
        if (input.IsKeyDown(Keys.Left))  angleY -= 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.Right)) angleY += 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.Up))    angleX -= 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.Down))  angleX += 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.W))     position.Y += 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.S))     position.Y -= 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.A))     position.X -= 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.D))     position.X += 1f * (float)e.Time;
        if (input.IsKeyDown(Keys.Q))     scale += 0.5f * (float)e.Time;
        if (input.IsKeyDown(Keys.E))     scale -= 0.5f * (float)e.Time;
        scale = Math.Max(scale, 0.1f);

        // Вычисляем матрицу модели на основе параметров
        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateScale(scale);                    // масштабирование
        model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angleX));
        model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angleY));
        model *= Matrix4.CreateTranslation(position);

        // Передаём в шейдер (например, uniform с location 0)
        GL.UniformMatrix4(1, false, ref model);
    }


    protected override void OnRenderFrame(FrameEventArgs e)
    {
        // Очистка экрана
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_program);
        GL.BindVertexArray(_vao);

        // Передаём матрицы model/view/projection через Uniform (см. далее)
        // Например:
        // GL.UniformMatrix4(modelLoc, false, ref model);
        // GL.UniformMatrix4(viewLoc, false, ref view);
        // GL.UniformMatrix4(projLoc, false, ref projection);

        // Рисуем (примитив - треугольники)
        GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

}


public class Program
{
    public static void Main()
    {
// В методе Main создаём и запускаем окно
        var nativeSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(800, 600),

            // Size = new OpenTK.Mathematics.Vector2i(800, 600),
            Title = "My OpenGL App"
        };
        using (var game = new Game(GameWindowSettings.Default, nativeSettings))
        {
            game.Run();
        }
    }
}