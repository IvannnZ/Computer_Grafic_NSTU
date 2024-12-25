#include "malloc.h"
#include <SDL2/SDL.h>
#include <algorithm>
#include <iostream>
//#include <stack>
#include <vector>

int size_x = 600;
int size_y = 600;

SDL_Window* window = NULL;
SDL_Renderer* renderer = NULL;


SDL_Color create_color(int r, int g, int b, int a)
{
    SDL_Color color;
    color.r = r;
    color.g = g;
    color.b = b;
    color.a = a;
    return color;
}

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

    static int orientation(const Point& p, const Point& q, const Point& r)
    {
        int val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (val == 0) return 0;
        return (val > 0) ? 1 : 2;
    }

    static bool onSegment(const Point& p, const Point& q, const Point& r)
    {
        return q.x <= std::max(p.x, r.x) && q.x >= std::min(p.x, r.x) &&
            q.y <= std::max(p.y, r.y) && q.y >= std::min(p.y, r.y);
    }

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

void refresh_screen()
{
    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
    SDL_RenderClear(renderer);
    SDL_RenderPresent(renderer);
}

void render()
{
    SDL_RenderPresent(renderer);
}

void Draw_line(const Line& line)
{
    SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
    SDL_RenderDrawLine(renderer, line.s().x, line.s().y, line.e().x, line.e().y);
}

void Draw_point(const Point& point)
{
    SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
    int X = 0;
    int Y = 5;
    int d = 3 - 2 * 5;

    while (Y >= X)
    {
        SDL_RenderDrawPoint(renderer, point.x + X, point.y + Y);
        SDL_RenderDrawPoint(renderer, point.x - X, point.y + Y);
        SDL_RenderDrawPoint(renderer, point.x + X, point.y - Y);
        SDL_RenderDrawPoint(renderer, point.x - X, point.y - Y);
        SDL_RenderDrawPoint(renderer, point.x + Y, point.y + X);
        SDL_RenderDrawPoint(renderer, point.x - Y, point.y + X);
        SDL_RenderDrawPoint(renderer, point.x + Y, point.y - X);
        SDL_RenderDrawPoint(renderer, point.x - Y, point.y - X);
        if (d <= 0)
        {
            d = d + 4 * X + 6;
        }
        else
        {
            d = d + 4 * (X - Y) + 10;
            Y--;
        }
        X++;
    }
}


bool comp(const Line& a, const Line& b)
{
    return a.length() < b.length();
}

void Triangulation(const std::vector<Point>& points)
{
    refresh_screen();
    for (const Point& point : points)
    {
        Draw_point(point);
    }
    if (points.size() < 2)
    {
        return;
    }
    std::vector<Line> all_lines;

    // Генерация всех возможных линий между точками
    for (size_t i = 0; i < points.size(); ++i)
    {
        for (size_t j = i + 1; j < points.size(); ++j)
        {
            all_lines.push_back(Line(points[i], points[j]));
        }
    }

    // Сортируем линии по длине
    std::sort(all_lines.begin(), all_lines.end(), comp);

    std::vector<Line> approve_lines;

    // Проверяем пересечения линий
    for (const Line& line_i : all_lines)
    {
        bool can_add = true;
        for (const Line& line_j : approve_lines)
        {
            if (line_i.intersects(line_j))
            {
                can_add = false;
                break;
            }
        }
        if (can_add)
        {
            approve_lines.push_back(line_i);
            Draw_line(line_i); // Отрисовываем линию
        }
    }

    render();
}


int main()
{
    window = SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED,
                              SDL_WINDOWPOS_CENTERED, size_x + 1,
                              size_y + 1, SDL_WINDOW_SHOWN);
    if (!window)
    {
        std::cerr << "Ошибка создания окна: " << SDL_GetError() << std::endl;
        SDL_Quit();
        return 1;
    }
    renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
    bool running = true;
    SDL_Event event;
    std::vector<Point> points;
    while (running)
    {
        while (SDL_PollEvent(&event))
        {
            if (event.type == SDL_QUIT)
            {
                running = false;
            }
            else if (event.type == SDL_MOUSEBUTTONDOWN)
            {
                points.push_back(Point(event.button.x, event.button.y));
                Triangulation(points);
            }
        }
    }
    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(window);
    SDL_Quit();
    return 0;
}
