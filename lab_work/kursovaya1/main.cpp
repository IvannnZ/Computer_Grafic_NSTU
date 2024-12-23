#include "My_grafcs.h"
#include <iostream>
#include <vector>

int main()
{
    int numSquares; // Количество квадратов в сетке
    int squareSize; // Размер одного квадрата

    std::cout << "Enter number of squares in grid: ";
    std::cin >> numSquares;
    std::cout << "Enter the size of one square: ";
    std::cin >> squareSize;

    std::cout << "1-dot 2-line 3-circle 4-triangle: ";
    int chouse;
    std::cin >> chouse;
    My_graphics window(numSquares, squareSize);
    window.refresh_screen();
    window.Draw_grid();


    switch (chouse)
    {
    case 1:
        {
            int pointX, pointY; // Координаты точки
            std::cout << "Enter the coordinates of the point (X and Y separated by a "
                "space): ";
            std::cin >> pointX >> pointY;
            window.Draw_point(pointX, pointY);
            break;
        }
    case 2:
        {
            std::cout << "1 - Draw_line_digital_differential_analyzer\n2 - "
                "DrawlineBresenham: ";
            std::cin >> chouse;
            int l_x_s, l_x_e, l_y_s, l_y_e;
            std::cout << "Enter coordinate line like l_x_s, l_y_s, l_x_e, l_y_e: ";
            std::cin >> l_x_s >> l_y_s >> l_x_e >> l_y_e;

            switch (chouse)
            {
            case 1:
                {
                    window.Draw_line_digital_differential_analyzer(l_x_s, l_y_s, l_x_e,
                                                                   l_y_e);
                    break;
                }
            case 2:
                {
                    window.DrawlineBresenham(l_x_s, l_y_s, l_x_e, l_y_e);
                    break;
                }
            default: break;;
            }
            break;
        }
    case 3:
        {
            std::cout << "Enter center circle x, y, and radius: ";
            int centerX, centerY, radius;
            std::cin >> centerX >> centerY >> radius;
            window.DrawCircleBresenham(centerX, centerY, radius);
            break;
        }
    case 4:
        {
            std::cout << "Enter coordinate triangle like this:\nx0 y0 x1 y1 x2 y2:\n";
            int x0, y0, x1, y1, x2, y2;
            std::cin >> x0 >> y0 >> x1 >> y1 >> x2 >> y2;

            window.DrawTriangleV2(x0, y0, x1, y1, x2, y2);
            break;
        }
    case 5:
        {
            window.DrawCircleBresenham(10, 10, 10, window.create_color(255, 0, 0, 255));
            window.DrawTriangle(5, 5, 5, 13, 15, 13, window.create_color(0, 0, 0, 255));
            break;
        }
    case 6:
        {
            //    int startX, startY;
            //    std::cout << "Enter starting point (x, y): ";
            //    std::cin >> startX >> startY;
            //
            //    int r, g, b, a;
            //    std::cout << "Enter fill color (R G B A): ";
            //    std::cin >> r >> g >> b >> a;
            //    SDL_Color fillColor = window.create_color(r, g, b, a);
            //
            //    std::cout << "Enter boundary color (R G B A): ";
            //    std::cin >> r >> g >> b >> a;
            //    SDL_Color boundaryColor = window.create_color(r, g, b, a);

            window.refresh_screen(window.create_color(0, 0, 0, 255));
            window.DrawCircleBresenham(10, 10, 5);
            window.DrawCircleBresenham(13, 10, 5);
            window.render();
            SDL_Delay(3000);
            std::cout << "a\n";
            window.FloodFill(10, 10, window.create_color(255, 0, 0, 0),
                             window.create_color(0, 0, 0, 255));
            break;
        }
    case 7:
        {
            window.DrawCircleBresenham(20, 20, 15);
            window.DrawTriangleShape(5, 25, 35, 25, 20, 40);
            window.FloodFill(20, 20, window.create_color(255, 2, 25, 255), window.create_color(0, 0, 0, 255));
        }
    case 8:
        {
            window.refresh_screen(window.create_color(0, 0, 0, 0));
            std::vector<point> points;

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
            // points.push_back({18, 2});
            // points.push_back({18, 8});
            // points.push_back({12, 16});
            // points.push_back({2, 5});

            // домик с обратной крышей
            // points.push_back({2,2});
            // points.push_back({18,2});
            // points.push_back({18,18});
            // points.push_back({12,8});
            // points.push_back({2, 16});

            // треугольник
            // points.push_back({5, 5});
            // points.push_back({15, 3});
            // points.push_back({3, 15});

            // галочка ей бы масштаб больше
            points.push_back({2, 2});
            points.push_back({19, 5});
            points.push_back({7, 7});
            points.push_back({5, 19});

            window.DrawPoligon(points);
        }
    default: break;
    }
    window.render();
    int a;
    std::cin >> a;

    return 0;
}
