using System;
using System.Numerics;

public struct Vec4
{
    public float x, y, z, w;

    // Конструкторы
    public Vec4(float xx, float yy, float zz, float ww = 1.0f)
    {
        x = xx;
        y = yy;
        z = zz;
        w = ww;
    }

    public Vec4(Vector3 xyz, float ww = 1.0f) : this(xyz.X, xyz.Y, xyz.Z, ww)
    {
    }
    
    public Vec4(Vec4 xyz) : this(xyz.x, xyz.y, xyz.z, xyz.w)
    {
    }

    // Операторы
    public static Vec4 operator +(Vec4 v1, Vec4 v2) => new Vec4(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    public static Vec4 operator -(Vec4 v1, Vec4 v2) => new Vec4(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    public static Vec4 operator *(Vec4 v, float f) => new Vec4(v.x * f, v.y * f, v.z * f);
    public static Vec4 operator /(Vec4 v, float f) => new Vec4(v.x / f, v.y / f, v.z / f);
    public static Vec4 operator -(Vec4 v) => v * -1;

    // Операторы присваивания
    public void Add(Vec4 v)
    {
        x += v.x;
        y += v.y;
        z += v.z;
    }

    public void Subtract(Vec4 v)
    {
        x -= v.x;
        y -= v.y;
        z -= v.z;
    }

    public void Multiply(float f)
    {
        x *= f;
        y *= f;
        z *= f;
    }

    public void Divide(float f)
    {
        x /= f;
        y /= f;
        z /= f;
    }

    // Векторные операции
    public static float Dot(Vec4 v1, Vec4 v2) => v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;

    public static Vec4 Cross(Vec4 a, Vec4 b) => new Vec4(
        a.y * b.z - a.z * b.y,
        a.z * b.x - a.x * b.z,
        a.x * b.y - a.y * b.x,
        0 // w=0 для векторного произведения
    );

    // Нормализация (только xyz)
    public void Normalize()
    {
        float length = Length();
        if (length > 0)
        {
            x /= length;
            y /= length;
            z /= length;
        }
    }

    public float Length() => (float)Math.Sqrt(x * x + y * y + z * z);

    // Преобразование в Vector3
    public Vector3 ToVector3() => new Vector3(x, y, z);

    // Пересечение плоскости с линией
    public static Vec4 IntersectPlane(Vec4 plane_p, Vec4 plane_n, Vec4 lineStart, Vec4 lineEnd, out float t)
    {
        plane_n.Normalize();
        float plane_d = -Dot(plane_n, plane_p);
        float ad = Dot(lineStart, plane_n);
        float bd = Dot(lineEnd, plane_n);
        t = (-plane_d - ad) / (bd - ad);

        Vec4 lineStartToEnd = lineEnd - lineStart;
        Vec4 lineToIntersect = lineStartToEnd * t;
        return lineStart + lineToIntersect;
    }

    // Для отладки
    public override string ToString() => $"({x:F2}, {y:F2}, {z:F2}, {w:F2})";
}


public class Mat4x4
{
    public float[,] m = new float[4, 4];

    // Конструкторы
    public Mat4x4()
    {
        // Матрица инициализируется нулями по умолчанию
    }

    public Mat4x4(float[,] values)
    {
        if (values.GetLength(0) != 4 || values.GetLength(1) != 4)
            throw new ArgumentException("Matrix must be 4x4");

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                m[i, j] = values[i, j];
            }
        }
    }

    // Матрица проекции
    public static Mat4x4 GetProjectionMatrix(int width = 1920, int height = 1080,
        float fNear = 0.1f, float fFar = 1000f,
        float fov = 90f)
    {
        const float PI = 3.1415926535f;
        float aspectRatio = (float)height / width;
        float fovRad = 1.0f / (float)Math.Tan(fov * 0.5f / 180.0f * PI);

        return new Mat4x4(new float[,]
        {
            { aspectRatio * fovRad, 0, 0, 0 },
            { 0, fovRad, 0, 0 },
            { 0, 0, fFar / (fFar - fNear), 1 },
            { 0, 0, -fFar * fNear / (fFar - fNear), 0 }
        });
    }

    // Матрицы вращения
    public static Mat4x4 GetRotationX(float angle)
    {
        Mat4x4 matrix = new Mat4x4();
        matrix.m[0, 0] = 1.0f;
        matrix.m[1, 1] = (float)Math.Cos(angle);
        matrix.m[1, 2] = (float)Math.Sin(angle);
        matrix.m[2, 1] = -(float)Math.Sin(angle);
        matrix.m[2, 2] = (float)Math.Cos(angle);
        matrix.m[3, 3] = 1.0f;
        return matrix;
    }

    public static Mat4x4 GetRotationY(float angle)
    {
        Mat4x4 matrix = new Mat4x4();
        matrix.m[0, 0] = (float)Math.Cos(angle);
        matrix.m[0, 2] = -(float)Math.Sin(angle);
        matrix.m[2, 0] = (float)Math.Sin(angle);
        matrix.m[1, 1] = 1.0f;
        matrix.m[2, 2] = (float)Math.Cos(angle);
        matrix.m[3, 3] = 1.0f;
        return matrix;
    }

    public static Mat4x4 GetRotationZ(float angle)
    {
        Mat4x4 matrix = new Mat4x4();
        matrix.m[0, 0] = (float)Math.Cos(angle);
        matrix.m[0, 1] = (float)Math.Sin(angle);
        matrix.m[1, 0] = -(float)Math.Sin(angle);
        matrix.m[1, 1] = (float)Math.Cos(angle);
        matrix.m[2, 2] = 1.0f;
        matrix.m[3, 3] = 1.0f;
        return matrix;
    }

    // Матрица "наведения" на точку
    public static Mat4x4 GetPointAtMatrix(Vec4 pos, Vec4 target, Vec4 up)
    {
        Vec4 newForward = target - pos;
        newForward.Normalize();

        Vec4 a = newForward * Vec4.Dot(up, newForward);
        Vec4 newUp = up - a;
        newUp.Normalize();

        Vec4 newRight = Vec4.Cross(newUp, newForward);

        Mat4x4 matrix = new Mat4x4();
        matrix.m[0, 0] = newRight.x;
        matrix.m[0, 1] = newRight.y;
        matrix.m[0, 2] = newRight.z;
        matrix.m[0, 3] = 0.0f;
        matrix.m[1, 0] = newUp.x;
        matrix.m[1, 1] = newUp.y;
        matrix.m[1, 2] = newUp.z;
        matrix.m[1, 3] = 0.0f;
        matrix.m[2, 0] = newForward.x;
        matrix.m[2, 1] = newForward.y;
        matrix.m[2, 2] = newForward.z;
        matrix.m[2, 3] = 0.0f;
        matrix.m[3, 0] = pos.x;
        matrix.m[3, 1] = pos.y;
        matrix.m[3, 2] = pos.z;
        matrix.m[3, 3] = 1.0f;
        return matrix;
    }

    // Инверсия матрицы (только для матриц вращения/перемещения)
    public void Invert()
    {
        Mat4x4 matrix = new Mat4x4();
        matrix.m[0, 0] = this.m[0, 0];
        matrix.m[0, 1] = this.m[1, 0];
        matrix.m[0, 2] = this.m[2, 0];
        matrix.m[0, 3] = 0.0f;
        matrix.m[1, 0] = this.m[0, 1];
        matrix.m[1, 1] = this.m[1, 1];
        matrix.m[1, 2] = this.m[2, 1];
        matrix.m[1, 3] = 0.0f;
        matrix.m[2, 0] = this.m[0, 2];
        matrix.m[2, 1] = this.m[1, 2];
        matrix.m[2, 2] = this.m[2, 2];
        matrix.m[2, 3] = 0.0f;
        matrix.m[3, 0] =
            -(this.m[3, 0] * matrix.m[0, 0] + this.m[3, 1] * matrix.m[1, 0] + this.m[3, 2] * matrix.m[2, 0]);
        matrix.m[3, 1] =
            -(this.m[3, 0] * matrix.m[0, 1] + this.m[3, 1] * matrix.m[1, 1] + this.m[3, 2] * matrix.m[2, 1]);
        matrix.m[3, 2] =
            -(this.m[3, 0] * matrix.m[0, 2] + this.m[3, 1] * matrix.m[1, 2] + this.m[3, 2] * matrix.m[2, 2]);
        matrix.m[3, 3] = 1.0f;
        this.m = matrix.m;
    }

    // Умножение матрицы на вектор
    public static Vec4 operator *(Mat4x4 m, Vec4 v)
    {
        Vec4 result = new Vec4(0, 0, 0, 0);
        result.x = v.x * m.m[0, 0] + v.y * m.m[1, 0] + v.z * m.m[2, 0] + v.w * m.m[3, 0];
        result.y = v.x * m.m[0, 1] + v.y * m.m[1, 1] + v.z * m.m[2, 1] + v.w * m.m[3, 1];
        result.z = v.x * m.m[0, 2] + v.y * m.m[1, 2] + v.z * m.m[2, 2] + v.w * m.m[3, 2];
        result.w = v.x * m.m[0, 3] + v.y * m.m[1, 3] + v.z * m.m[2, 3] + v.w * m.m[3, 3];

        if (result.w != 0)
        {
            result.x /= result.w;
            result.y /= result.w;
            result.z /= result.w;
        }

        return result;
    }

    public static Vec4 operator *(Vec4 v, Mat4x4 m)
    {
        Vec4 result = new Vec4(0, 0, 0, 0);
        result.x = v.x * m.m[0, 0] + v.y * m.m[1, 0] + v.z * m.m[2, 0] + v.w * m.m[3, 0];
        result.y = v.x * m.m[0, 1] + v.y * m.m[1, 1] + v.z * m.m[2, 1] + v.w * m.m[3, 1];
        result.z = v.x * m.m[0, 2] + v.y * m.m[1, 2] + v.z * m.m[2, 2] + v.w * m.m[3, 2];
        result.w = v.x * m.m[0, 3] + v.y * m.m[1, 3] + v.z * m.m[2, 3] + v.w * m.m[3, 3];

        if (result.w != 0)
        {
            result.x /= result.w;
            result.y /= result.w;
            result.z /= result.w;
        }

        return result;
    }

    // Умножение Матрици на матрицу
    public static Mat4x4 operator *(Mat4x4 a, Mat4x4 b)
    {
        Mat4x4 result = new Mat4x4();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                result.m[i, j] = 0;
                for (int k = 0; k < 4; k++)
                {
                    result.m[i, j] += a.m[i, k] * b.m[k, j];
                }
            }
        }

        return result;
    }

    // Для отладки
    public override string ToString()
    {
        return string.Format(
            "[{0:F2}, {1:F2}, {2:F2}, {3:F2}]\n" +
            "[{4:F2}, {5:F2}, {6:F2}, {7:F2}]\n" +
            "[{8:F2}, {9:F2}, {10:F2}, {11:F2}]\n" +
            "[{12:F2}, {13:F2}, {14:F2}, {15:F2}]",
            m[0, 0], m[0, 1], m[0, 2], m[0, 3],
            m[1, 0], m[1, 1], m[1, 2], m[1, 3],
            m[2, 0], m[2, 1], m[2, 2], m[2, 3],
            m[3, 0], m[3, 1], m[3, 2], m[3, 3]);
    }
}