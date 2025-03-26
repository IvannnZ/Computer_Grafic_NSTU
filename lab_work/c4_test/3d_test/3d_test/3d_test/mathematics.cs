using System;

public class Vec3
{
    public float X, Y, Z;

    public Vec3(float x = 0, float y = 0, float z = 0)
    {
        X = x; Y = y; Z = z;
    }

    public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vec3 operator -(Vec3 a, Vec3 b) => new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vec3 operator *(Vec3 a, float scalar) => new Vec3(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public void Norm()
    {
        float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        if (length > 0)
        {
            X /= length; Y /= length; Z /= length;
        }
    }

    public static float DotProd(Vec3 a, Vec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    public static Vec3 CrossProd(Vec3 a, Vec3 b) =>
        new Vec3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
}

public class Mat4x4
{
    public float[,] M = new float[4, 4];

    public static Mat4x4 GetProjMat(float width, float height, float near, float far, float fov)
    {
        float aspect = width / height;
        float fovRad = 1.0f / (float)Math.Tan(fov * 0.5f / 180.0f * Math.PI);
        Mat4x4 matrix = new Mat4x4();
        matrix.M[0, 0] = aspect * fovRad;
        matrix.M[1, 1] = fovRad;
        matrix.M[2, 2] = far / (far - near);
        matrix.M[3, 2] = (-far * near) / (far - near);
        matrix.M[2, 3] = 1.0f;
        return matrix;
    }

    public static Mat4x4 GetRotY(float angle)
    {
        Mat4x4 matrix = new Mat4x4();
        matrix.M[0, 0] = (float)Math.Cos(angle);
        matrix.M[0, 2] = (float)Math.Sin(angle);
        matrix.M[2, 0] = -(float)Math.Sin(angle);
        matrix.M[2, 2] = (float)Math.Cos(angle);
        matrix.M[1, 1] = 1;
        matrix.M[3, 3] = 1;
        return matrix;
    }

    public static Mat4x4 GetRotZ(float angle)
    {
        Mat4x4 matrix = new Mat4x4();
        matrix.M[0, 0] = (float)Math.Cos(angle);
        matrix.M[0, 1] = (float)Math.Sin(angle);
        matrix.M[1, 0] = -(float)Math.Sin(angle);
        matrix.M[1, 1] = (float)Math.Cos(angle);
        matrix.M[2, 2] = 1;
        matrix.M[3, 3] = 1;
        return matrix;
    }

    public static Vec3 operator *(Mat4x4 matrix, Vec3 vector)
    {
        Vec3 result = new Vec3();
        result.X = vector.X * matrix.M[0, 0] + vector.Y * matrix.M[0, 1] + vector.Z * matrix.M[0, 2] + matrix.M[0, 3];
        result.Y = vector.X * matrix.M[1, 0] + vector.Y * matrix.M[1, 1] + vector.Z * matrix.M[1, 2] + matrix.M[1, 3];
        result.Z = vector.X * matrix.M[2, 0] + vector.Y * matrix.M[2, 1] + vector.Z * matrix.M[2, 2] + matrix.M[2, 3];
        return result;
    }
}