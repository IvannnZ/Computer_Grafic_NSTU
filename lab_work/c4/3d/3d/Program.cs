using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SFML_3D_Viewer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SFML 3D Viewer");
            Console.WriteLine("Controls:");
            Console.WriteLine("W, A, S, D - Move camera");
            Console.WriteLine("Q, E - Zoom in/out");
            Console.WriteLine("1, 2, 3, 4, 5 - Load shapes (Tetrahedron, Cube, Octahedron, Dodecahedron, Icosahedron)");
            Console.WriteLine("ESC - Exit");

            var window = new RenderWindow(new VideoMode(800, 600), "3D Shape Viewer");
            window.SetFramerateLimit(60);
            
            // Enable depth testing for proper 3D rendering
            window.SetVerticalSyncEnabled(true);

            // Camera position
            Vector3f cameraPosition = new Vector3f(0, 0, -5);
            float cameraSpeed = 0.1f;
            float zoomSpeed = 0.1f;

            // Current shape
            Shape3D? currentShape = null;
            float rotationSpeed = 1.0f;
            float time = 0;

            // Load default shape (cube)
            try
            {
                currentShape = LoadObjFile("cube.obj");
                if (currentShape != null)
                {
                    currentShape.Color = Color.Green;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading shape: {ex.Message}");
            }

            window.Closed += (sender, e) => window.Close();
            window.KeyPressed += (sender, e) =>
            {
                if (e.Code == Keyboard.Key.Escape) window.Close();

                // Shape selection
                if (e.Code == Keyboard.Key.Num1) LoadShape("1.obj", Color.Red, ref currentShape);
                if (e.Code == Keyboard.Key.Num2) LoadShape("2.obj", Color.Green, ref currentShape);
                if (e.Code == Keyboard.Key.Num3) LoadShape("3.obj", Color.Blue, ref currentShape);
                if (e.Code == Keyboard.Key.Num4) LoadShape("4.obj", Color.Yellow, ref currentShape);
                if (e.Code == Keyboard.Key.Num5) LoadShape("5.obj", Color.Magenta, ref currentShape);
                if (e.Code == Keyboard.Key.Num6) LoadShape("cube.obj", Color.Green, ref currentShape);
                if (e.Code == Keyboard.Key.Num7) LoadShape("teapot.obj", Color.Blue, ref currentShape);

                // Camera movement
                if (e.Code == Keyboard.Key.W) cameraPosition.Y += cameraSpeed;
                if (e.Code == Keyboard.Key.S) cameraPosition.Y -= cameraSpeed;
                if (e.Code == Keyboard.Key.A) cameraPosition.X += cameraSpeed;
                if (e.Code == Keyboard.Key.D) cameraPosition.X -= cameraSpeed;

                // Zoom
                if (e.Code == Keyboard.Key.Q) cameraPosition.Z += zoomSpeed;
                if (e.Code == Keyboard.Key.E) cameraPosition.Z -= zoomSpeed;
            };

            Clock clock = new Clock();
            while (window.IsOpen)
            {
                window.DispatchEvents();

                // Update time
                float deltaTime = clock.Restart().AsSeconds();
                time += deltaTime;

                // Update shape position (sinusoidal movement)
                if (currentShape != null)
                {
                    currentShape.Position = new Vector3f(
                        currentShape.Position.X,
                        (float)Math.Sin(time) * 1.5f,
                        currentShape.Position.Z
                    );

                    // Rotate shape
                    currentShape.Rotation = new Vector3f(
                        currentShape.Rotation.X,
                        currentShape.Rotation.Y + rotationSpeed * deltaTime,
                        currentShape.Rotation.Z
                    );
                }

                window.Clear(Color.Black);

                // Draw shape
                if (currentShape != null)
                {
                    currentShape.Draw(window, cameraPosition);
                }

                window.Display();
            }
        }

        static void LoadShape(string filename, Color color, ref Shape3D? shape)
        {
            try
            {
                var newShape = LoadObjFile(filename);
                if (newShape != null)
                {
                    newShape.Color = color;
                    newShape.Position = shape?.Position ?? new Vector3f(0, 0, 0);
                    newShape.Rotation = shape?.Rotation ?? new Vector3f(0, 0, 0);
                    shape = newShape;
                    Console.WriteLine($"Loaded {filename}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
            }
        }

        static Shape3D? LoadObjFile(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File {filename} not found");
                return null;
            }

            var vertices = new List<Vector3f>();
            var faces = new List<int[]>();

            foreach (var line in File.ReadAllLines(filename))
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        vertices.Add(new Vector3f(
                            float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                            float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
                            float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture))
                        );
                    }
                }
                else if (line.StartsWith("f "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var face = new int[parts.Length - 1];
                    for (int i = 1; i < parts.Length; i++)
                    {
                        // Handle cases like "f v1/vt1/vn1 v2/vt2/vn2 ..."
                        var vertexPart = parts[i].Split('/')[0];
                        face[i - 1] = int.Parse(vertexPart) - 1; // OBJ indices are 1-based
                    }
                    faces.Add(face);
                }
            }

            if (vertices.Count == 0 || faces.Count == 0)
            {
                Console.WriteLine($"No valid geometry found in {filename}");
                return null;
            }

            return new Shape3D(vertices, faces);
        }
    }

    class Shape3D
    {
        public List<Vector3f> Vertices { get; }
        public List<int[]> Faces { get; }
        public Vector3f Position { get; set; }
        public Vector3f Rotation { get; set; }
        public Color Color { get; set; }

        public Shape3D(List<Vector3f> vertices, List<int[]> faces)
        {
            Vertices = vertices;
            Faces = faces;
            Position = new Vector3f(0, 0, 0);
            Rotation = new Vector3f(0, 0, 0);
            Color = Color.White;
        }

        public void Draw(RenderWindow window, Vector3f cameraPosition)
        {
            // Simple 3D to 2D projection
            foreach (var face in Faces)
            {
                if (face.Length < 3) continue; // We need at least 3 vertices for a face

                // Create SFML convex shape for each face
                var convex = new ConvexShape((uint)face.Length);
                convex.FillColor = new Color(Color.R, Color.G, Color.B, 150);
                convex.OutlineThickness = 1;
                convex.OutlineColor = Color;

                // Project each vertex
                for (int i = 0; i < face.Length; i++)
                {
                    if (face[i] < 0 || face[i] >= Vertices.Count) continue;

                    var vertex = Vertices[face[i]];

                    // Apply transformations
                    vertex = RotateVertex(vertex, Rotation);
                    vertex = new Vector3f(
                        vertex.X + Position.X,
                        vertex.Y + Position.Y,
                        vertex.Z + Position.Z
                    );

                    // Simple perspective projection
                    float scale = 200.0f; // Scaling factor
                    float distance = 5.0f; // Distance to projection plane

                    // Adjust for camera position
                    vertex = new Vector3f(
                        vertex.X - cameraPosition.X,
                        vertex.Y - cameraPosition.Y,
                        vertex.Z - cameraPosition.Z
                    );

                    // Avoid division by zero
                    float z = vertex.Z == 0 ? 0.001f : vertex.Z;

                    // Perspective projection
                    Vector2f projected = new Vector2f(
                        vertex.X * distance / z * scale + window.Size.X / 2,
                        -vertex.Y * distance / z * scale + window.Size.Y / 2
                    );

                    convex.SetPoint((uint)i, projected);
                }

                window.Draw(convex);
            }
        }

        private Vector3f RotateVertex(Vector3f vertex, Vector3f rotation)
        {
            // Rotate around Y axis
            float angleY = rotation.Y * (float)Math.PI / 180.0f;
            float cosY = (float)Math.Cos(angleY);
            float sinY = (float)Math.Sin(angleY);

            Vector3f rotated = new Vector3f(
                vertex.X * cosY - vertex.Z * sinY,
                vertex.Y,
                vertex.X * sinY + vertex.Z * cosY
            );

            // Rotate around X axis
            float angleX = rotation.X * (float)Math.PI / 180.0f;
            float cosX = (float)Math.Cos(angleX);
            float sinX = (float)Math.Sin(angleX);

            rotated = new Vector3f(
                rotated.X,
                rotated.Y * cosX - rotated.Z * sinX,
                rotated.Y * sinX + rotated.Z * cosX
            );

            return rotated;
        }
    }
}