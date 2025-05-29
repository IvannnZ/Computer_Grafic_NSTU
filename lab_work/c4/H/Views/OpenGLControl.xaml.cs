// Views/OpenGLControl.xaml.cs
using SharpGL;
using SharpGL.WPF; // Добавлено
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Photo3DEditor.Views
{
    public partial class OpenGLControl : UserControl
    {
        public SharpGL.WPF.OpenGLControl GLControl => openGLControl;
        public OpenGLControl()
        {
            InitializeComponent();
            openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        }

        // Добавлен обработчик события
        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
        }
    }
}