using System;
using System.Numerics;

namespace Custom3DEngine
{
    public struct Vector4D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4D(float x, float y, float z, float w = 1.0f)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4D(Vector3 xyz, float w = 1.0f) : this(xyz.X, xyz.Y, xyz.Z, w) { }

        public static Vector4D operator +(Vector4D a, Vector4D b) => 
            new Vector4D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector4D operator -(Vector4D a, Vector4D b) => 
            new Vector4D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector4D operator *(Vector4D v, float scalar) => 
            new Vector4D(v.X * scalar, v.Y * scalar, v.Z * scalar);

        public static Vector4D operator /(Vector4D v, float scalar) => 
            new Vector4D(v.X / scalar, v.Y / scalar, v.Z / scalar);

        public static Vector4D operator -(Vector4D v) => v * -1f;

        public static float Dot(Vector4D a, Vector4D b) => 
            a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector4D Cross(Vector4D a, Vector4D b) => 
            new Vector4D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X,
                0f
            );

        public void Normalize()
        {
            float len = Length();
            if (len > float.Epsilon)
            {
                X /= len;
                Y /= len;
                Z /= len;
            }
        }

        public float Length() => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector3 ToVector3() => new Vector3(X, Y, Z);

        public static Vector4D PlaneIntersection(
            Vector4D planePoint, 
            Vector4D planeNormal,
            Vector4D lineStart,
            Vector4D lineEnd,
            out float t)
        {
            planeNormal.Normalize();
            float planeD = -Dot(planeNormal, planePoint);
            float ad = Dot(lineStart, planeNormal);
            float bd = Dot(lineEnd, planeNormal);
            t = (-planeD - ad) / (bd - ad);

            Vector4D lineDir = lineEnd - lineStart;
            Vector4D intersectOffset = lineDir * t;
            return lineStart + intersectOffset;
        }

        public override string ToString() => 
            $"({X:F2}, {Y:F2}, {Z:F2}, {W:F2})";
    }

    public class Matrix4x4
    {
        private float[,] _data = new float[4, 4];
        public float this[int row, int col]
        {
            get => _data[row, col];
            set => _data[row, col] = value;
        }

        public static Matrix4x4 Identity()
        {
            var m = new Matrix4x4();
            m[0, 0] = m[1, 1] = m[2, 2] = m[3, 3] = 1f;
            return m;
        }

        public static Matrix4x4 Projection(
            int width, 
            int height,
            float near = 0.1f,
            float far = 1000f,
            float fov = 90f)
        {
            const float PI = 3.1415926535f;
            float aspect = (float)height / width;
            float fovScale = 1f / (float)Math.Tan(fov * 0.5f * PI / 180f);

            var m = new Matrix4x4();
            m[0, 0] = aspect * fovScale;
            m[1, 1] = fovScale;
            m[2, 2] = far / (far - near);
            m[3, 2] = -far * near / (far - near);
            m[2, 3] = 1f;
            return m;
        }

        public static Matrix4x4 RotationX(float angle)
        {
            var m = Identity();
            m[1, 1] = (float)Math.Cos(angle);
            m[1, 2] = (float)Math.Sin(angle);
            m[2, 1] = -(float)Math.Sin(angle);
            m[2, 2] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4x4 RotationY(float angle)
        {
            var m = Identity();
            m[0, 0] = (float)Math.Cos(angle);
            m[0, 2] = -(float)Math.Sin(angle);
            m[2, 0] = (float)Math.Sin(angle);
            m[2, 2] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4x4 RotationZ(float angle)
        {
            var m = Identity();
            m[0, 0] = (float)Math.Cos(angle);
            m[0, 1] = (float)Math.Sin(angle);
            m[1, 0] = -(float)Math.Sin(angle);
            m[1, 1] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4x4 Scale(float x, float y, float z)
        {
            var m = new Matrix4x4();
            m[0, 0] = x;
            m[1, 1] = y;
            m[2, 2] = z;
            m[3, 3] = 1f;
            return m;
        }

        public static Matrix4x4 LookAt(Vector4D pos, Vector4D target, Vector4D up)
        {
            Vector4D forward = (target - pos);
            forward.Normalize();

            Vector4D right = Vector4D.Cross(up, forward);
            right.Normalize();

            Vector4D newUp = Vector4D.Cross(forward, right);

            var m = Identity();
            m[0, 0] = right.X; m[0, 1] = right.Y; m[0, 2] = right.Z;
            m[1, 0] = newUp.X; m[1, 1] = newUp.Y; m[1, 2] = newUp.Z;
            m[2, 0] = forward.X; m[2, 1] = forward.Y; m[2, 2] = forward.Z;
            m[3, 0] = pos.X; m[3, 1] = pos.Y; m[3, 2] = pos.Z;
            return m;
        }

        public void Invert()
        {
            var m = new Matrix4x4();
            m[0, 0] = this[0, 0]; m[0, 1] = this[1, 0]; m[0, 2] = this[2, 0];
            m[1, 0] = this[0, 1]; m[1, 1] = this[1, 1]; m[1, 2] = this[2, 1];
            m[2, 0] = this[0, 2]; m[2, 1] = this[1, 2]; m[2, 2] = this[2, 2];
            m[3, 0] = -(this[3, 0] * m[0, 0] + this[3, 1] * m[1, 0] + this[3, 2] * m[2, 0]);
            m[3, 1] = -(this[3, 0] * m[0, 1] + this[3, 1] * m[1, 1] + this[3, 2] * m[2, 1]);
            m[3, 2] = -(this[3, 0] * m[0, 2] + this[3, 1] * m[1, 2] + this[3, 2] * m[2, 2]);
            m[3, 3] = 1f;
            _data = m._data;
        }

        public static Vector4D operator *(Matrix4x4 m, Vector4D v)
        {
            Vector4D r = new Vector4D();
            r.X = v.X * m[0, 0] + v.Y * m[1, 0] + v.Z * m[2, 0] + v.W * m[3, 0];
            r.Y = v.X * m[0, 1] + v.Y * m[1, 1] + v.Z * m[2, 1] + v.W * m[3, 1];
            r.Z = v.X * m[0, 2] + v.Y * m[1, 2] + v.Z * m[2, 2] + v.W * m[3, 2];
            r.W = v.X * m[0, 3] + v.Y * m[1, 3] + v.Z * m[2, 3] + v.W * m[3, 3];

            if (r.W != 0)
            {
                r.X /= r.W;
                r.Y /= r.W;
                r.Z /= r.W;
            }
            return r;
        }
        public static Vector4D operator *( Vector4D v, Matrix4x4 m)
        {
            Vector4D r = new Vector4D();
            r.X = v.X * m[0, 0] + v.Y * m[1, 0] + v.Z * m[2, 0] + v.W * m[3, 0];
            r.Y = v.X * m[0, 1] + v.Y * m[1, 1] + v.Z * m[2, 1] + v.W * m[3, 1];
            r.Z = v.X * m[0, 2] + v.Y * m[1, 2] + v.Z * m[2, 2] + v.W * m[3, 2];
            r.W = v.X * m[0, 3] + v.Y * m[1, 3] + v.Z * m[2, 3] + v.W * m[3, 3];

            if (r.W != 0)
            {
                r.X /= r.W;
                r.Y /= r.W;
                r.Z /= r.W;
            }
            return r;
        }

        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            var m = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                        m[i, j] += a[i, k] * b[k, j];
            return m;
        }

        public override string ToString() => 
            $"[{_data[0, 0]:F2}, {_data[0, 1]:F2}, ...]";
    }
}