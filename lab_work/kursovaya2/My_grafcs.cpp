#include "My_grafcs.h"


SDL_Color My_graphics::create_color(int r, int g, int b, int a)
{
    SDL_Color color;
    color.r = r;
    color.g = g;
    color.b = b;
    color.a = a;
    return color;
}


My_graphics::My_graphics(int size_x, int size_y)
    : size_x(size_x), size_y(size_y)
{
    if (SDL_Init(SDL_INIT_VIDEO) != 0)
    {
        throw "Ошибка инициализации SDL: ";
    }

    window = SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED,
                              SDL_WINDOWPOS_CENTERED, size_x + 1,
                              size_y + 1, SDL_WINDOW_SHOWN);
    if (!window)
    {
        SDL_Quit();
        throw "Ошибка создания окна: ";
    }

    renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
    if (!renderer)
    {
        SDL_DestroyWindow(window);
        SDL_Quit();
        throw "Ошибка создания рендерера: ";
    }
}

My_graphics::~My_graphics()
{
    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(window);
    SDL_Quit();
}

bool comp(const Line& a, const Line& b)
{
    return a.length() < b.length();
}

void My_graphics::Triangulation(const std::vector<Point>& points) const {


    refresh_screen();
    for(const Point &point : points)
    {
        Draw_point(point);
    }
    if (points.size() < 2) {
        return;
    }
    std::vector<Line> all_lines;

    for (size_t i = 0; i < points.size(); ++i) {
        for (size_t j = i + 1; j < points.size(); ++j) {
            all_lines.push_back(Line(points[i], points[j]));
        }
    }

    std::sort(all_lines.begin(), all_lines.end(), comp);

    std::vector<Line> approve_lines;

    for (const Line& line_i : all_lines) {
        bool can_add = true;
        for (const Line& line_j : approve_lines) {
            if (line_i.intersects(line_j)) {
                can_add = false;
                break;
            }
        }
        if (can_add) {
            approve_lines.push_back(line_i);
            Draw_line(line_i);
        }
    }

    render();
}



void My_graphics::Draw_point(const Point &point) const
{
    Draw_point(point, create_color(0, 255, 0, 255));
}

void My_graphics::Draw_point(const Point& point, SDL_Color color) const
{
    SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
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


void My_graphics::refresh_screen() const
{
    refresh_screen(create_color(0, 0, 0, 255));
}

void My_graphics::refresh_screen(SDL_Color color) const
{
    SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
    SDL_RenderClear(renderer);
    SDL_RenderPresent(renderer);
}

void My_graphics::render() const
{
    SDL_RenderPresent(renderer);
}

void My_graphics::Draw_line(int x1, int y1, int x2, int y2) const
{
    Draw_line(x1, y1, x2, y2, create_color(255, 0, 0, 255));
}

void My_graphics::Draw_line(int x1, int y1, int x2, int y2, SDL_Color color) const
{
    SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
    SDL_RenderDrawLine(renderer, x1, y1, x2, y2);
}

void My_graphics::Draw_line(const Line& line) const
{
    Draw_line(line, create_color(255, 0, 0, 255));
}

void My_graphics::Draw_line(const Line& line, SDL_Color color) const
{
    SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
    SDL_RenderDrawLine(renderer, line.s().x, line.s().y, line.e().x, line.e().y);
}
