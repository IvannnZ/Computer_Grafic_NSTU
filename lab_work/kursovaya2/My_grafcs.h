#include <vector>
#include <SDL2/SDL.h>
#include <algorithm>
#include <iostream>

struct Point
{
    int x;
    int y;

    Point(int x = 0, int y = 0) : x(x), y(y)
    {
    }
};

class Line
{
private:
    Point start;
    Point end;

public:
    Line(const Point& p1, const Point& p2) : start(p1), end(p2)
    {
    }

    double length() const
    {
        int dx = end.x - start.x;
        int dy = end.y - start.y;
        return std::sqrt(dx * dx + dy * dy);
    }

    Point s() const { return start; }

    Point e() const { return end; }

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
