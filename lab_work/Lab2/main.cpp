#include "My_grafcs.h"
#include <iostream>


int main() {

  int numSquares=20;     // Количество квадратов в сетке
  int squareSize=20;     // Размер одного квадрата

  My_graphics window(numSquares, squareSize);
  window.refresh_screen();
  window.Draw_grid();
  window.Draw_point(1,1);
//  window.DrawlineBresenham(1, 1,20, 7);
  window.render();
  int a;
  std::cin >> a;

  return 0;
}




// void Draw_grid(SDL_Renderer *renderer, int numSquares, int squareSize);
//
// void Draw_point(SDL_Renderer *renderer, int x, int y, int squareSize,
//                int numSquares);
//
// void Draw_line_digital_differential_analyzer(SDL_Renderer *renderer, int x_s,
// int y_s, int x_e, int y_e,
//               int squareSize, int numSquares);
//
// void DrawlineBresenham(SDL_Renderer *renderer, int x_s, int y_s, int x_e,
//                        int y_e, int squareSize, int numSquares);
//
// void DrawCircleBresenham(SDL_Renderer *renderer, int centerX, int centerY,
//                          int radius, int squareSize, int numSquares);



//std::cout << "Enter number of squares in grid: ";
//  std::cin >> numSquares;
//std::cout << "Enter the size of one square: ";
//  std::cin >> squareSize;

//std::cout << "1-dot 2-line 3-circle";
//  int chouse;
//  std::cin >> chouse;





/*
  switch (chouse) {
  case 1:
    int pointX, pointY; // Координаты точки
    std::cout << "Enter the coordinates of the point (X and Y separated by a "
                 "space): ";
    std::cin >> pointX >> pointY;
    window.Draw_point(pointX, pointY);
  case 2:

std::cout << "1 - Draw_line_digital_differential_analyzer\n 2 - "
             "DrawlineBresenham";
std::cin >> chouse;
int l_x_s, l_x_e, l_y_s, l_y_e;
std::cout << "Enter coordinate line like l_x_s, l_y_s, l_x_e, l_y_e: ";
std::cin >> l_x_s >> l_y_s >> l_x_e >> l_y_e;


switch (chouse) {
case 1:
  window.Draw_line_digital_differential_analyzer(l_x_s, l_y_s, l_x_e,
                                                 l_y_e);
case 2:
  window.DrawlineBresenham(l_x_s, l_y_s, l_x_e, l_y_e);
}
case 3:
std::cout << "Enter center circle x, y, and radius: ";
int centerX, centerY, radius;
std::cin >> centerX >> centerY >> radius;
window.DrawCircleBresenham(centerX, centerY, radius);
}*/
