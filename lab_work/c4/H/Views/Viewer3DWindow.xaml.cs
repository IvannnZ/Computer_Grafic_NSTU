using System.Windows;
using System.Windows.Input;
using SharpGL;
using Photo3DEditor.Models;
using Photo3DEditor.Services;
using SharpGL.WPF;
using System.Windows.Controls;
using System.Windows.Media;
using OpenTK.Mathematics;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using MathNet.Numerics.Distributions;

namespace Photo3DEditor.Views
{
    public partial class Viewer3DWindow : Window
    {
        private readonly DataService _dataService = App.DataService;

        private float _rotationX = 20;
        private float _rotationY = -30;
        private float _zoom = 5.0f;
        private Point _lastMousePos;
        private bool _isDragging;
        private float _translateX = 0.0f;
        private float _translateY = 0.0f;


        private Vector3 _cameraTarget = Vector3.Zero;
        private Vector3 _cameraPosition = new(0, 0, 5);
        private Vector3 _cameraUp = Vector3.UnitY;

        private bool _fillTriangles = false;
        private bool _isPointdraw = true;
        private bool _isSmoth = false;
        private bool isObj = ImageWindow.isFromObj;
        private Vector3 _lightDirection = new(0, 1, 0);



        private Vector3 _rotationCenter = Vector3.Zero;

        public Viewer3DWindow()
        {
            InitializeComponent();

            GLView.MouseMove += GLView_MouseMove;
            GLView.MouseDown += GLView_MouseDown;
            GLView.MouseUp += GLView_MouseUp;
            GLView.MouseWheel += GLView_MouseWheel;

            this.KeyDown += Viewer3DWindow_KeyDown;
            this.Focusable = true;
            this.Focus(); 


            this.Left = 0;
            this.Top = 0;
            this.Width = App.weigth;
            this.Height = App.height;

            _dataService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(DataService.SelectedPoint))
                {
                    Refresh();
                }
            };
        }

        private void GLView_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            var lookAt = _cameraTarget;
            var position = _cameraPosition;
            var viewMatrix = CreateLookAtMatrix(position, lookAt, _cameraUp);
            gl.LoadMatrix(viewMatrix);

            gl.Translate(_translateX, _translateY, -_zoom);

            gl.Translate(_rotationCenter.X, _rotationCenter.Y, _rotationCenter.Z);
            gl.Rotate(_rotationX, 1.0f, 0.0f, 0.0f);
            gl.Rotate(_rotationY, 0.0f, 1.0f, 0.0f);
            gl.Translate(-_rotationCenter.X, -_rotationCenter.Y, -_rotationCenter.Z);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_MULTISAMPLE);


            gl.Color(0.678f, 1.0f, 0.184f);
            gl.LineWidth(2.0f);

            var triangles = !ImageWindow.isFromObj
    ? new ObservableCollection<MeshTriangle>(TriangulationService.Triangulate(_dataService.Points))
    : _dataService.Triangles;

            //---------------------------------------------------------


            var vertexNormals = new Dictionary<Point3D, Vector3>();

            foreach (var triangle in triangles)
            {
                var a = new Vector3((float)triangle.A.X, (float)triangle.A.Y, (float)triangle.A.Z) / 100.0f;
                var b = new Vector3((float)triangle.B.X, (float)triangle.B.Y, (float)triangle.B.Z) / 100.0f;
                var c = new Vector3((float)triangle.C.X, (float)triangle.C.Y, (float)triangle.C.Z) / 100.0f;

                var ab = b - a;
                var ac = c - a;
                var normal = Vector3.Cross(ab, ac);
                normal.Normalize();

                foreach (var point in new[] { triangle.A, triangle.B, triangle.C })
                {
                    if (!vertexNormals.ContainsKey(point))
                        vertexNormals[point] = Vector3.Zero;

                    vertexNormals[point] += normal;
                }
            }

            foreach (var point in vertexNormals.Keys.ToList())
            {
                vertexNormals[point].Normalize();
            }


            //---------------------------------------------------------


            if(_isSmoth)
            {
                foreach (var triangle in triangles)
                {
                    var a = new Vector3((float)triangle.A.X, (float)triangle.A.Y, (float)triangle.A.Z) / 100.0f;
                    var b = new Vector3((float)triangle.B.X, (float)triangle.B.Y, (float)triangle.B.Z) / 100.0f;
                    var c = new Vector3((float)triangle.C.X, (float)triangle.C.Y, (float)triangle.C.Z) / 100.0f;

                    var ab = b - a;
                    var ac = c - a;
                    var normal = Vector3.Cross(ab, ac);
                    normal.Normalize();

                    var brightness = Vector3.Dot(normal, _lightDirection) + 1;
 
                    var color = (brightness / 2f) * (brightness / 2f);

                    if (_fillTriangles)
                    {
                        gl.Color(color, color, color);
                        gl.Begin(OpenGL.GL_TRIANGLES);
                    }
                    else
                    {
                        gl.Color(0.678f, 1.0f, 0.184f);
                        gl.Begin(OpenGL.GL_LINE_LOOP);
                    }

                    gl.Vertex(a.X, a.Y, a.Z);
                    gl.Vertex(b.X, b.Y, b.Z);
                    gl.Vertex(c.X, c.Y, c.Z);
                    gl.End();
                }
            }
            else
            {
                foreach (var triangle in triangles)
                {
                    var a = new Vector3((float)triangle.A.X, (float)triangle.A.Y, (float)triangle.A.Z) / 100.0f;
                    var b = new Vector3((float)triangle.B.X, (float)triangle.B.Y, (float)triangle.B.Z) / 100.0f;
                    var c = new Vector3((float)triangle.C.X, (float)triangle.C.Y, (float)triangle.C.Z) / 100.0f;

                    if (_fillTriangles)
                    {
                        gl.Begin(OpenGL.GL_TRIANGLES);


                        float ambientStrength = 0.15f; 
                        float softnessFactor = 0.7f;   
                        float gamma = 1.8f;               

                        float SmoothBrightness(float rawBrightness)
                        {
                            rawBrightness = ambientStrength + (1 - ambientStrength) * rawBrightness;
                            rawBrightness = MathF.Pow(rawBrightness, softnessFactor);
                            return rawBrightness / (rawBrightness + 0.4f);
                        }

                        Vector3 normalA = vertexNormals[triangle.A];
                        float brightnessA = (Vector3.Dot(normalA, _lightDirection) + 1) * 0.5f;
                        brightnessA = SmoothBrightness(brightnessA);
                        float colorA = MathF.Pow(brightnessA, 1.0f / gamma);
                        gl.Color(colorA, colorA, colorA);
                        gl.Vertex(a.X, a.Y, a.Z);

                        Vector3 normalB = vertexNormals[triangle.B];
                        float brightnessB = (Vector3.Dot(normalB, _lightDirection) + 1) * 0.5f;
                        brightnessB = SmoothBrightness(brightnessB);
                        float colorB = MathF.Pow(brightnessB, 1.0f / gamma);
                        gl.Color(colorB, colorB, colorB);
                        gl.Vertex(b.X, b.Y, b.Z);

                        Vector3 normalC = vertexNormals[triangle.C];
                        float brightnessC = (Vector3.Dot(normalC, _lightDirection) + 1) * 0.5f;
                        brightnessC = SmoothBrightness(brightnessC);
                        float colorC = MathF.Pow(brightnessC, 1.0f / gamma);
                        gl.Color(colorC, colorC, colorC);
                        gl.Vertex(c.X, c.Y, c.Z);

                        gl.End();
                    }
                    else
                    {
                        gl.Color(0.678f, 1.0f, 0.184f);
                        gl.Begin(OpenGL.GL_LINE_LOOP);
                        gl.Vertex(a.X, a.Y, a.Z);
                        gl.Vertex(b.X, b.Y, b.Z);
                        gl.Vertex(c.X, c.Y, c.Z);
                        gl.End();
                    }
                }
            }


            if (_isPointdraw)
            {
                gl.Color(1.0f, 0.0f, 0.0f);
                gl.PointSize(10.0f);
                gl.Begin(OpenGL.GL_POINTS);
                foreach (var point in _dataService.Points)
                {
                    gl.Vertex(point.X / 100.0, point.Y / 100.0, point.Z / 100.0);
                }
                gl.End();

                if (_dataService.SelectedPoint != null)
                {
                    gl.Color(0.0f, 1.0f, 1.0f);
                    gl.PointSize(15.0f);
                    gl.Begin(OpenGL.GL_POINTS);
                    gl.Vertex(
                        _dataService.SelectedPoint.X / 100.0,
                        _dataService.SelectedPoint.Y / 100.0,
                        _dataService.SelectedPoint.Z / 100.0
                    );

                    gl.End();
                }
            }
            gl.Flush();
        }

        private void Viewer3DWindow_KeyDown(object sender, KeyEventArgs e)
        {
            const float step = 0.1f;

            switch (e.Key)
            {
                case Key.Left:
                case Key.A:
                    _translateX += step;
                    break;
                case Key.Right:
                case Key.D:
                    _translateX -= step;
                    break;
                case Key.Up:
                case Key.W:
                    _translateY -= step;
                    break;
                case Key.Down:
                case Key.S:
                    _translateY += step;
                    break;
                case Key.Tab:
                    _fillTriangles = !_fillTriangles;
                    break;
                case Key.Q:
                    _isPointdraw = !_isPointdraw;
                    break;
                case Key.R:
                    _isSmoth = !_isSmoth;
                    break;
            }

            Refresh();
        }

        private double[] CreateLookAtMatrix(Vector3 eye, Vector3 center, Vector3 up)
        {
            Vector3 f = Vector3.Normalize(center - eye);
            Vector3 s = Vector3.Normalize(Vector3.Cross(f, up));
            Vector3 u = Vector3.Cross(s, f);

            return new double[]
            {
        s.X, u.X, -f.X, 0.0f,
        s.Y, u.Y, -f.Y, 0.0f,
        s.Z, u.Z, -f.Z, 0.0f,
        -Vector3.Dot(s, eye), -Vector3.Dot(u, eye), Vector3.Dot(f, eye), 1.0f
            };
        }
        private void GLView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var mousePos = e.GetPosition(GLView);
            SelectPoint(mousePos);
            Refresh();
        }

        private void GLView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                _isDragging = true;
                _lastMousePos = e.GetPosition(GLView);

                if (_dataService.SelectedPoint != null)
                {
                    _rotationCenter = new Vector3(
                        (float)(_dataService.SelectedPoint.X / 100.0),
                        (float)(_dataService.SelectedPoint.Y / 100.0),
                        (float)(_dataService.SelectedPoint.Z / 100.0));
                }
                else
                {
                    _rotationCenter = GetWorldPointFromMouse(_lastMousePos);
                }
            }
        }


        private void GLView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                _isDragging = false;
        }

        private void GLView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || e.RightButton != MouseButtonState.Pressed) return;

            Point currentPos = e.GetPosition(GLView);
            var delta = new Point(currentPos.X - _lastMousePos.X, currentPos.Y - _lastMousePos.Y);

            _rotationY += (float)delta.X;
            _rotationX += (float)delta.Y;

            _lastMousePos = currentPos;
            Refresh();
        }

        private void GLView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoomFactor = e.Delta > 0 ? 0.9f : 1.1f;

            // Приближаем к SelectedPoint, если есть
            if (_dataService.SelectedPoint != null)
            {
                var target = new Vector3(
                    (float)_dataService.SelectedPoint.X / 100.0f,
                    (float)_dataService.SelectedPoint.Y / 100.0f,
                    (float)_dataService.SelectedPoint.Z / 100.0f
                );

                var direction = _cameraPosition - target;
                _cameraPosition = target + direction * zoomFactor;
                _cameraTarget = target;
            }
            else
            {
                // Центр сцены
                _cameraPosition.Z *= zoomFactor;
            }

            Refresh();
        }



        public void Refresh()
        {
            GLView.InvalidateVisual();
        }

        private void SelectPoint(Point mousePosition)
        {
            var gl = GLView.OpenGL;
            gl.PushAttrib(OpenGL.GL_ALL_ATTRIB_BITS);
            gl.PushMatrix();

            try
            {
                double[] modelView = new double[16];
                double[] projection = new double[16];
                int[] viewport = new int[4];

                gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelView);
                gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);
                gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

                double startX = 0, startY = 0, startZ = 0;
                double endX = 0, endY = 0, endZ = 0;

                gl.UnProject(mousePosition.X, viewport[3] - mousePosition.Y, 0, modelView, projection, viewport, ref startX, ref startY, ref startZ);
                gl.UnProject(mousePosition.X, viewport[3] - mousePosition.Y, 1, modelView, projection, viewport, ref endX, ref endY, ref endZ);

                double minDistance = double.MaxValue;
                Point3D? closestPoint = null;

                foreach (var point in _dataService.Points)
                {
                    var p = new Vector3(
                        (float)(point.X / 100.0),
                        (float)(point.Y / 100.0),
                        (float)(point.Z / 100.0));

                    float distance = DistanceToLine(
                        new Vector3((float)startX, (float)startY, (float)startZ),
                        new Vector3((float)endX, (float)endY, (float)endZ),
                        p);

                    if (distance < 0.1f && distance < minDistance)
                    {
                        closestPoint = point;
                        minDistance = distance;
                    }
                }

                _dataService.SelectedPoint = closestPoint;
            }
            finally
            {
                gl.PopMatrix();
                gl.PopAttrib();
            }
        }

        private Vector3 GetWorldPointFromMouse(Point mouse)
        {
            var gl = GLView.OpenGL;

            double[] modelView = new double[16];
            double[] projection = new double[16];
            int[] viewport = new int[4];

            gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelView);
            gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projection);
            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

            double x = 0, y = 0, z = 0;
            gl.UnProject(mouse.X, viewport[3] - mouse.Y, 0.5, modelView, projection, viewport, ref x, ref y, ref z);

            return new Vector3((float)x, (float)y, (float)z);
        }

        private float DistanceToLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDir = lineEnd - lineStart;
            float lineLength = lineDir.Length;
            lineDir = Vector3.Normalize(lineDir);

            Vector3 v = point - lineStart;
            float d = Vector3.Dot(v, lineDir);
            d = Math.Clamp(d, 0, lineLength);

            Vector3 closestPoint = lineStart + lineDir * d;
            return (point - closestPoint).Length;
        }
    }
}
