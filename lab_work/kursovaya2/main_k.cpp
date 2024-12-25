#include "malloc.h"
#include <SDL2/SDL.h>
#include <algorithm>
#include <iostream>
//#include <stack>
#include <vector>

int grid = 30;
int size = 20;

SDL_Window* window = NULL;
SDL_Renderer* renderer = NULL;
SDL_Color** pixels;


struct point
{
    int x;
    int y;
};

SDL_Color create_color(int r, int g, int b, int a)
{
    SDL_Color color;
    color.r = r;
    color.g = g;
    color.b = b;
    color.a = a;
    return color;
}

bool IsSameColor(SDL_Color c0, SDL_Color c1)
{
    return c0.r == c1.r && c0.g == c1.g && c0.b == c1.b && c0.a == c1.a;
}

void draw_pix(int x, int y)
{
    if (x >= 0 && y >= 0 && x <= grid && y <= grid)
    {
        pixels[x][y] = create_color(0, 255, 0, 255);
    }
}

void draw_pix(int x, int y, SDL_Color color)
{
    if (x >= 0 && y >= 0 && x <= grid && y <= grid)
    {
        pixels[x][y] = color;
    }
}

void draw_grid()
{
    SDL_SetRenderDrawColor(renderer, 150, 150, 150, 150);
    for (int i = 0; i <= grid; i++)
    {
        SDL_RenderDrawLine(renderer, i * size, 0, i * size, grid * size);
        SDL_RenderDrawLine(renderer, 0, i * size, grid * size, i * size);
    }
}

void render()
{
    int numSquares = grid;
    int squareSize = size;
    for (int x = 0; x < numSquares; ++x)
    {
        for (int y = 0; y < numSquares; ++y)
        {
            SDL_SetRenderDrawColor(renderer, pixels[x][y].r, pixels[x][y].g,
                                   pixels[x][y].b, pixels[x][y].a);

            SDL_Rect rect = {
                (x - 1) * squareSize,
                numSquares * squareSize - y * squareSize, squareSize,
                squareSize
            };
            SDL_RenderFillRect(renderer, &rect);
        }
    }
    draw_grid();
    SDL_RenderPresent(renderer);
}

inline void line_Help(int x1, int y, bool swap)
{
    if (swap)
    {
        draw_pix(y, x1);
    }
    else
    {
        draw_pix(x1, y);
    }
}

void draw_bres(int x1, int y1, int x2, int y2)
{
    if (x1 == x2 && y1 == y2)
    {
        draw_pix(x1, y1);
        return;
    }
    int dx = abs(x2 - x1), dy = abs(y2 - y1);
    bool swap = false;
    if (dy > dx)
    {
        swap = true;
        std::swap(x1, y1);
        std::swap(x2, y2);
        std::swap(dx, dy);
    }
    int sx = x2 >= x1 ? 1 : -1;
    int sy = y2 >= y1 ? 1 : -1;
    float t = grid * (float)dy / dx;
    float w = grid - t;
    float d = grid / 2.0;
    int i = dx + 1;
    if (!t)
    {
        while (i--)
        {
            line_Help(x1, y1, swap);
            x1 += sx;
        }
        return;
    }
    line_Help(x1, y1, swap);
    while (--i)
    {
        if (d >= w)
        {
            d -= w;
            y1 += sy;
            line_Help(x1, y1, swap);
            x1 += sx;
        }
        else
        {
            d += t;
            x1 += sx;
        }
        line_Help(x1, y1, swap);
    }
}

void draw_tr(int x0, int y0, int x1, int y1, int x2, int y2)
{
    draw_bres(x0, y0, x1, y1);
    draw_bres(x1, y1, x2, y2);
    draw_bres(x2, y2, x0, y0);
}

void DrawCircleBresenham(int centerX, int centerY, int radius)
{
    int x = 0;
    int y = radius;
    int d = 3 - 2 * radius;

    while (y >= x)
    {
        draw_pix(centerX + x, centerY + y);
        draw_pix(centerX - x, centerY + y);
        draw_pix(centerX + x, centerY - y);
        draw_pix(centerX - x, centerY - y);
        draw_pix(centerX + y, centerY + x);
        draw_pix(centerX - y, centerY + x);
        draw_pix(centerX + y, centerY - x);
        draw_pix(centerX - y, centerY - x);

        if (d <= 0)
        {
            d = d + 4 * x + 6;
        }
        else
        {
            d = d + 4 * (x - y) + 10;
            y--;
        }
        x++;
    }
}

void refresh_screen()
{
    SDL_Color color;
    color.r = 0;
    color.g = 0;
    color.b = 0;
    color.a = 255;
    for (int x = 0; x < grid; ++x)
    {
        for (int y = 0; y < grid; ++y)
        {
            pixels[x][y] = color;
        }
    }
    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
    SDL_RenderClear(renderer);
}

void FloodFill(int x, int y, SDL_Color fillColor, SDL_Color baseColor)
{
    if (x < 0 || y < 0 || x >= grid || y >= grid)
    {
        return;
    }
    SDL_Color p = pixels[x][y];
    if (IsSameColor(pixels[x + 1][y], baseColor))
    {
        draw_pix(x + 1, y, fillColor);
        render();
        SDL_Delay(100);
        FloodFill(x + 1, y, fillColor, baseColor);
    }
    if (IsSameColor(pixels[x - 1][y], baseColor))
    {
        draw_pix(x - 1, y, fillColor);
        render();
        SDL_Delay(100);
        FloodFill(x - 1, y, fillColor, baseColor);
    }
    if (IsSameColor(pixels[x][y + 1], baseColor))
    {
        draw_pix(x, y + 1, fillColor);
        render();
        SDL_Delay(100);
        FloodFill(x, y + 1, fillColor, baseColor);
    }
    if (IsSameColor(pixels[x][y - 1], baseColor))
    {
        draw_pix(x, y - 1, fillColor);
        render();
        SDL_Delay(100);
        FloodFill(x, y - 1, fillColor, baseColor);
    }
}


void horisontal_lines(std::vector<point> points)
{
    std::sort(points.begin(), points.end(), [](const point& a, const point& b) { return a.x < b.x; });
    int x0 = -1;
    for (std::vector<point>::iterator a = points.begin(); a != points.end(); ++a)
    {
        if (x0 < 0)
        {
            x0 = a->x;
        }
        else
        {
            draw_bres(x0, a->y, a->x, a->y);
            x0 = -1;
            render();
            SDL_Delay(100);
        }
    }
}


void DrawPoligon(std::vector<point>& points)
{
    if (points.size() < 3) { return; }
    points.push_back(points[0]);
    int min_y = points[0].y, max_y = points[0].y;
    for (int i = 0; i < points.size(); ++i)
    {
        if (min_y > points[i].y) { min_y = points[i].y; }
        if (max_y < points[i].y) { max_y = points[i].y; }
    }
    int x0, y0, x1, y1, x2, y2;
    for (int i = min_y; i < max_y; ++i)
    {
        std::vector<point> lines;
        for (int j = 1; j < points.size(); ++j)
        {
            if ((points[j].y >= i && points[j - 1].y <= i) || (points[j].y <= i && points[j - 1].y >= i))
            {
                //x0 + (i - y0) * (x2 - x0) / (y2 - y0)
                x0 = points[j - 1].x;
                y0 = points[j - 1].y;
                x1 = points[j].x;
                y1 = points[j].y;
                //x2 = points[j < (points.size() - 1) ? j + 1 : 1].x;
                // следующая точка, и либо у нас нет выхда за массив, либо нет, и тогда мы берём 2 элемент( первый и последний это один и тот же элемент)
                y2 = points[j < (points.size() - 1) ? j + 1 : 1].y;
                // следующая точка, и либо у нас нет выхда за массив, либо нет, и тогда мы берём 2 элемент( первый и последний это один и тот же элемент)

                if (y0 != y1)
                {
                    if (y1 == i && ((y1 < y0 && y1 > y2) || (y1 > y0 && y1 < y2))) { continue; }
                    lines.push_back({x0 + (i - y0) * (x1 - x0) / (y1 - y0), i});
                }
            }
        }
        horisontal_lines(lines);
        lines.clear();
    }
    render();
}


int main()
{
    pixels = (SDL_Color**)malloc(sizeof(SDL_Color*) * grid);
    for (int i = 0; i < grid; ++i)
    {
        pixels[i] = (SDL_Color*)malloc(sizeof(SDL_Color) * grid);
    }
    window = SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED,
                              SDL_WINDOWPOS_CENTERED, grid * size + 1,
                              grid * size + 1, SDL_WINDOW_SHOWN);
    if (!window)
    {
        std::cerr << "Ошибка создания окна: " << SDL_GetError() << std::endl;
        SDL_Quit();
        return 1;
    }
    renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
    SDL_Event e;
    SDL_PollEvent(&e);
    refresh_screen();
    draw_grid();

    std::vector<point> points;

    // просто ввод пока не встетим отрицательную координату
    // std::cout << "Enter point coordinate point until enter - coordinate:\n";
    // int x, y;
    // std::cin >> x >> y;
    // while (x >= 0 && y >= 0)
    // {
    //     std::cout<<"\nnext:";
    //     points.push_back({x, y});
    //     std::cin >> x >> y;
    // }

    // домик
    // points.push_back({2, 2});
    // points.push_back({28, 2});
    // points.push_back({28, 8});
    // points.push_back({2
    // 0, 24});
    // points.push_back({2, 10});

    // домик с обратной крышей
    points.push_back({2,2});
    points.push_back({28,2});
    points.push_back({28,28});
    points.push_back({18,12});
    points.push_back({2, 20});

    // треугольник
    // points.push_back({7, 7});
    // points.push_back({28, 5});
    // points.push_back({5, 28});

    // галочка ей бы масштаб больше
    // points.push_back({7, 7});
    // points.push_back({28, 5});
    // points.push_back({13, 13});
    // points.push_back({5, 28});

    DrawPoligon(points);

    render();
    int a;
    std::cout << "Чтобы выйти нажмите любой символ\n";
    std::cin >> a;
    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(window);
    SDL_Quit();
    return 0;
}
