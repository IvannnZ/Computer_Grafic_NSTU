#include <vector>
#include <SDL2/SDL.h>
#include <algorithm>
#include <iostream>
#include <stack>

struct Point
{
    int x;
    int y;

    // Конструктор для инициализации координат
    Point(int x = 0, int y = 0) : x(x), y(y)
    {
    }

    // Статическая функция создания точки
    // static Point create_point(int x, int y) {
    //     return Point(x, y);
    // }
};

class Line
{
private:
    Point start;
    Point end;

    // Вспомогательная функция для определения ориентации
    static int orientation(const Point& p, const Point& q, const Point& r)
    {
        int val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (val == 0) return 0; // Коллинеарные точки
        return (val > 0) ? 1 : 2; // 1 - по часовой стрелке, 2 - против часовой стрелки
    }

    // Проверка, находится ли точка q на отрезке pr
    static bool onSegment(const Point& p, const Point& q, const Point& r)
    {
        return q.x <= std::max(p.x, r.x) && q.x >= std::min(p.x, r.x) &&
            q.y <= std::max(p.y, r.y) && q.y >= std::min(p.y, r.y);
    }

public:
    // Конструктор от 4 координат
    Line(int x0, int y0, int x1, int y1) : start(x0, y0), end(x1, y1)
    {
    }

    // Конструктор от двух точек
    Line(const Point& p1, const Point& p2) : start(p1), end(p2)
    {
    }

    // Метод для вычисления длины линии
    double length() const
    {
        int dx = end.x - start.x;
        int dy = end.y - start.y;
        return std::sqrt(dx * dx + dy * dy);
    }

    Point s() const { return start; }

    Point e() const { return end; }
    // Метод для проверки пересечения с другой линией
    bool intersects(const Line& other) const
    {
        double x1 = start.x, y1 = start.y;
        double x2 = end.x, y2 = end.y;
        double x3 = other.start.x, y3 = other.start.y;
        double x4 = other.end.x, y4 = other.end.y;

        double denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (denominator == 0) return false;

        double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denominator;
        double u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denominator;

        return (t > 0 && t < 1 && u > 0 && u < 1);


        // const Point& x0 = start;
        // const Point& y0 = end;
        // const Point& x1 = other.start;
        // const Point& y1 = other.end;
        //
        // int o1 = orientation(x0, y0, x1);
        // int o2 = orientation(x0, y0, y1);
        // int o3 = orientation(x1, y1, x0);
        // int o4 = orientation(x1, y1, y0);
        //
        // // Общий случай: линии пересекаются
        // if (o1 != o2 && o3 != o4) return true;
        //
        // // Частные случаи: проверка касания
        // if (o1 == 0 && onSegment(x0, x1, y0)) return true;
        // if (o2 == 0 && onSegment(x0, y1, y0)) return true;
        // if (o3 == 0 && onSegment(x1, x0, y1)) return true;
        // if (o4 == 0 && onSegment(x1, y0, y1)) return true;
        //
        // return false;
    }
};

class My_graphics
{
private:
    int size_x, size_y;
    SDL_Window* window;
    SDL_Renderer* renderer;

public:
    My_graphics(int numSquares, int squareSize);

    ~My_graphics();

    void Triangulation(const std::vector<Point>& points) const;

    static SDL_Color create_color(int r, int g, int b, int a);

    void refresh_screen() const;

    void refresh_screen(SDL_Color color) const;

    void render() const;

    void Draw_line(int x1, int y1, int x2, int y2) const;
    void Draw_line(int x1, int y1, int x2, int y2, SDL_Color color) const;
    void Draw_line(const Line& line) const;
    void Draw_line(const Line& line, SDL_Color color) const;

    void Draw_point(const Point& point) const;
    void Draw_point(const Point& point, SDL_Color color) const;
};
