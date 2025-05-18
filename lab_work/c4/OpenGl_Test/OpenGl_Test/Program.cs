using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using OpenTK.Windowing.GraphicsLibraryFramework;

public class MainWindow : GameWindow
{
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _shaderProgram;

    // Пример вершин (можно заменить на данные из .obj)
    private readonly float[] _vertices = {
        // позиция           // цвет
         0.0f,  0.5f, 0.0f,  1f, 0f, 0f,
        -0.5f, -0.5f, 0.0f,  0f, 1f, 0f,
         0.5f, -0.5f, 0.0f,  0f, 0f, 1f,
    };

    private readonly int[] _indices = {
        0, 1, 2
    };

    public MainWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
    {
        
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices, BufferUsageHint.StaticDraw);

        // Позиции
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // Цвета
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Шейдеры
        var vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec3 aColor;
            out vec3 vertexColor;
            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
                vertexColor = aColor;
            }
        ";

        var fragmentShaderSource = @"
            #version 330 core
            in vec3 vertexColor;
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(vertexColor, 1.0);
            }
        ";

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteVertexArray(_vao);
        GL.DeleteProgram(_shaderProgram);
    }
}

class Program
{
    static void Main(string[] args)
    {
        
        
        
        var gws = GameWindowSettings.Default;
        var nws = new NativeWindowSettings()
        {
            Title = "OpenGL + C# (OpenTK)",
            Size = new Vector2i(800, 600)
        };

        using var window = new MainWindow(gws, nws);
        window.Run();
    }
}


// using OpenTK.Windowing.Desktop;
// using OpenTK.Windowing.Common;
// using OpenTK.Windowing.GraphicsLibraryFramework;
// using OpenTK.Graphics.OpenGL4; // Для GL.*
// using OpenTK.Mathematics;       // Для Vector3, Matrix4 и т.п.
//
//
// // Определяем класс приложения
// class Game : GameWindow
// {
//     public Game(GameWindowSettings gwSettings, NativeWindowSettings nwSettings)
//         : base(gwSettings, nwSettings)
//     {
//         
//     }
//
//     protected override void OnLoad()
//     {
//         // Пример строки шейдера (или загрузка из файла)
//         string vertexSource = @"
//         #version 330 core
//         layout(location = 0) in vec3 aPos;
//         layout(location = 1) in vec3 aNormal;
//         uniform mat4 model;
//         uniform mat4 view;
//         uniform mat4 projection;
//         out vec3 Normal;
//         void main()
//         {
//             gl_Position = projection * view * model * vec4(aPos, 1.0);
//             Normal = aNormal;
//         }";
//         string fragmentSource = @"
//         #version 330 core
//         in vec3 Normal;
//         out vec4 FragColor;
//         void main()
//         {
//             // Простое затенение по нормали (для наглядности)
//             vec3 color = normalize(Normal) * 0.5 + 0.5;
//             FragColor = vec4(color, 1.0);
//         }";
//
//         // Создаём и компилируем вершинный шейдер
//         int vertexShader = GL.CreateShader(ShaderType.VertexShader);
//         GL.ShaderSource(vertexShader, vertexSource);
//         GL.CompileShader(vertexShader);
//         // (Здесь хорошо бы проверить статус компиляции через GL.GetShader(...))
//
//         // Фрагментный шейдер
//         int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
//         GL.ShaderSource(fragmentShader, fragmentSource);
//         GL.CompileShader(fragmentShader);
//
//         // Линкуем программу
//         int shaderProgram = GL.CreateProgram();
//         GL.AttachShader(shaderProgram, vertexShader);
//         GL.AttachShader(shaderProgram, fragmentShader);
//         GL.LinkProgram(shaderProgram);
//
//         // После успешной линковки можно удалить шейдеры:
//         GL.DetachShader(shaderProgram, vertexShader);
//         GL.DetachShader(shaderProgram, fragmentShader);
//         GL.DeleteShader(vertexShader);
//         GL.DeleteShader(fragmentShader);
//
//         
//         
//         // Сохраним ID программы для использования при отрисовке
//         _program = shaderProgram;
//
//         // По умолчанию включаем тест глубины, чтобы правильно рисовать 3D
//         GL.Enable(EnableCap.DepthTest);
//     }
//
//
//     protected override void OnUpdateFrame(FrameEventArgs args)
//     {
//         // Обработка ввода, обновление логики (вращение и т.д.)
//     }
//
//     protected override void OnRenderFrame(FrameEventArgs args)
//     {
//         // Отрисовка кадра
//         SwapBuffers(); // показать изображение
//     }
// }
//
//
// public class Program
// {
//     public static void Main()
//     {
// // В методе Main создаём и запускаем окно
//         var nativeSettings = new NativeWindowSettings()
//         {
//             ClientSize = new Vector2i(800, 600),
//
//             // Size = new OpenTK.Mathematics.Vector2i(800, 600),
//             Title = "My OpenGL App"
//         };
//         using (var game = new Game(GameWindowSettings.Default, nativeSettings))
//         {
//             game.Run();
//         }
//     }
// }