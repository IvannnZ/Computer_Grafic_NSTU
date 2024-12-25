#include "My_grafcs.h"
#include <iostream>
#include <vector>

int main()
{
    My_graphics window(500, 500);
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
                window.Triangulation(points);
            }
        }
    }
    int a;
    std::cin >> a;

    return 0;
}
