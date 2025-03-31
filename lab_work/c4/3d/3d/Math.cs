using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;

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

public static class Matrix4x4Extensions
{
    public static Vec4 Transform(this Matrix4x4 m, Vec4 v)
    {
        return new Vec4(
            v.x * m.M11 + v.y * m.M21 + v.z * m.M31 + v.w * m.M41,
            v.x * m.M12 + v.y * m.M22 + v.z * m.M32 + v.w * m.M42,
            v.x * m.M13 + v.y * m.M23 + v.z * m.M33 + v.w * m.M43,
            v.x * m.M14 + v.y * m.M24 + v.z * m.M34 + v.w * m.M44
        );
    }
}


public class Mat4x4
{
    public float[,] m = new float[4, 4];

    public Mat4x4()
    {
    }

    public Mat4x4(float[,] values)
    {
        for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
            m[i, j] = values[i, j];
    }

    public static Mat4x4 GetProjectionMatrix(int width = 1920, int height = 1080,
        float fNear = 0.1f, float fFar = 1000f,
        float fovDegrees = 90f)
    {
        float aspectRatio = (float)height / width;
        float fovRad = fovDegrees * MathF.PI / 180f;
        float tanHalfFov = MathF.Tan(fovRad / 2f);

        return new Mat4x4(new float[4, 4]
        {
            { aspectRatio / tanHalfFov, 0, 0, 0 },
            { 0, 1 / tanHalfFov, 0, 0 },
            { 0, 0, fFar / (fFar - fNear), 1 },
            { 0, 0, -fFar * fNear / (fFar - fNear), 0 }
        });
    }

    public static Mat4x4 GetRotationX(float angle)
    {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        return new Mat4x4(new float[4, 4]
        {
            { 1, 0, 0, 0 },
            { 0, cos, sin, 0 },
            { 0, -sin, cos, 0 },
            { 0, 0, 0, 1 }
        });
    }

    public static Mat4x4 GetMatchtabe(float scaleX, float scaleY, float scaleZ, Vec4 point )
    {
        point = new Vec4(0.5f, 0.5f, 0.5f);
        // 1. Перенос точки в начало координат
        Mat4x4 translateToOrigin = new Mat4x4(new float[4, 4]
        {
            { 1, 0, 0, -point.x },
            { 0, 1, 0, -point.y },
            { 0, 0, 1, -point.z },
            { 0, 0, 0, 1 }
        });

        // 2. Матрица масштабирования
        Mat4x4 scaleMatrix = new Mat4x4(new float[4, 4]
        {
            { scaleX, 0, 0, 0 },
            { 0, scaleY, 0, 0 },
            { 0, 0, scaleZ, 0 },
            { 0, 0, 0, 1 }
        });

        // 3. Обратный перенос
        Mat4x4 translateBack = new Mat4x4(new float[4, 4]
        {
            { 1, 0, 0, point.x },
            { 0, 1, 0, point.y },
            { 0, 0, 1, point.z },
            { 0, 0, 0, 1 }
        });

        // Комбинируем преобразования: T⁻¹ * S * T
        return translateBack * scaleMatrix * translateToOrigin;
    }

    public static Mat4x4 GetRotationY(float angle)
    {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        return new Mat4x4(new float[4, 4]
        {
            { cos, 0, -sin, 0 },
            { 0, 1, 0, 0 },
            { sin, 0, cos, 0 },
            { 0, 0, 0, 1 }
        });
    }

    public static Mat4x4 GetRotationZ(float angle)
    {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        return new Mat4x4(new float[4, 4]
        {
            { cos, sin, 0, 0 },
            { -sin, cos, 0, 0 },
            { 0, 0, 1, 0 },
            { 0, 0, 0, 1 }
        });
    }

    public static Mat4x4 GetPointAtMatrix(Vec4 pos, Vec4 target, Vec4 up)
    {
        Vec4 newForward = (target - pos);
        newForward.Normalize();
        Vec4 a = newForward * Vec4.Dot(up, newForward);
        Vec4 newUp = (up - a);
        newUp.Normalize();
        Vec4 newRight = Vec4.Cross(newUp, newForward);

        return new Mat4x4(new float[4, 4]
        {
            { newRight.x, newRight.y, newRight.z, 0 },
            { newUp.x, newUp.y, newUp.z, 0 },
            { newForward.x, newForward.y, newForward.z, 0 },
            { pos.x, pos.y, pos.z, 1 }
        });
    }

    public void Invert()
    {
        float[,] result = new float[4, 4];

        // Транспонирование поворотной части
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            result[i, j] = m[j, i];

        // Обратный перевод
        result[3, 0] = -(m[3, 0] * result[0, 0] + m[3, 1] * result[1, 0] + m[3, 2] * result[2, 0]);
        result[3, 1] = -(m[3, 0] * result[0, 1] + m[3, 1] * result[1, 1] + m[3, 2] * result[2, 1]);
        result[3, 2] = -(m[3, 0] * result[0, 2] + m[3, 1] * result[1, 2] + m[3, 2] * result[2, 2]);
        result[3, 3] = 1f;

        m = result;
    }

    public static Vec4 operator *(Vec4 v, Mat4x4 m)
    {
        Vec4 result = new Vec4(
            v.x * m.m[0, 0] + v.y * m.m[1, 0] + v.z * m.m[2, 0] + v.w * m.m[3, 0],
            v.x * m.m[0, 1] + v.y * m.m[1, 1] + v.z * m.m[2, 1] + v.w * m.m[3, 1],
            v.x * m.m[0, 2] + v.y * m.m[1, 2] + v.z * m.m[2, 2] + v.w * m.m[3, 2],
            v.x * m.m[0, 3] + v.y * m.m[1, 3] + v.z * m.m[2, 3] + v.w * m.m[3, 3]
        );

        if (result.w != 0)
        {
            result.x /= result.w;
            result.y /= result.w;
            result.z /= result.w;
        }

        return result;
    }

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
}


