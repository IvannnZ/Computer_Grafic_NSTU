#include "My_grafcs.h"
#include "iostream"
#include <SDL2/SDL.h>


SDL_Color My_graphics ::create_color(int r, int g, int b, int a) {
  SDL_Color color;
  color.r = r;
  color.g = g;
  color.b = b;
  color.a = a;
  return color;
}

My_graphics ::My_graphics(int numSquares, int squareSize)
    : numSquares(numSquares), squareSize(squareSize) {

  if (SDL_Init(SDL_INIT_VIDEO) != 0) {
    throw "Ошибка инициализации SDL: ";
  }

  window = SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED,
                            SDL_WINDOWPOS_CENTERED, numSquares * squareSize + 1,
                            numSquares * squareSize + 1, SDL_WINDOW_SHOWN);
  if (!window) {
    SDL_Quit();
    throw "Ошибка создания окна: ";
  }

  renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
  if (!renderer) {
    SDL_DestroyWindow(window);
    SDL_Quit();
    throw "Ошибка создания рендерера: ";
  }
}

My_graphics::~My_graphics() {
  SDL_DestroyRenderer(renderer);
  SDL_DestroyWindow(window);
  SDL_Quit();
}

void My_graphics ::Draw_grid() { Draw_grid(create_color(255, 255, 255, 255)); }

void My_graphics ::Draw_grid(SDL_Color color) {
  SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

  for (int i = 0; i <= numSquares; i++) {
    SDL_RenderDrawLine(renderer, i * squareSize, 0, i * squareSize,
                       numSquares * squareSize);
    SDL_RenderDrawLine(renderer, 0, i * squareSize, numSquares * squareSize,
                       i * squareSize);
  }
}

void My_graphics ::Draw_point(int x, int y) {
  Draw_point(x, y, create_color(0, 255, 0, 255));
}

void My_graphics ::Draw_point(int x, int y, SDL_Color color) {
  SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
  x--;
  y--;
  SDL_Rect rect = {x * squareSize,
                   numSquares * squareSize - (y + 1) * squareSize, squareSize,
                   squareSize};
  SDL_RenderFillRect(renderer, &rect);

  //  SDL_Delay(5);
  //  render();
}

void My_graphics ::Draw_line_digital_differential_analyzer(int x_s, int y_s,
                                                           int x_e, int y_e) {
  Draw_line_digital_differential_analyzer(x_s, y_s, x_e, y_e,
                                          create_color(0, 255, 0, 255));
}

void My_graphics ::Draw_line_digital_differential_analyzer(int x_s, int y_s,
                                                           int x_e, int y_e,
                                                           SDL_Color color) {

  SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
  int dx = abs(x_e - x_s);
  int dy = abs(y_e - y_s);

  int steps = std::max(dx, dy);

  float x_inc = (float)dx / (float)steps;
  float y_inc = (float)dy / (float)steps;

  float x = (float)x_s;
  float y = (float)y_s;

  for (int i = 0; i <= steps; i++) {
    Draw_point(static_cast<int>(x), static_cast<int>(y), color);
    x += x_inc;
    y += y_inc;
  }
}

void My_graphics::DrawlineBresenham(int x_s, int y_s, int x_e, int y_e) {
  DrawlineBresenham(x_s, y_s, x_e, y_e, create_color(0, 255, 0, 255));
}

void My_graphics::DrawlineBresenham(int x_s, int y_s, int x_e, int y_e,
                                    SDL_Color color) {

  SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

  int dx = abs(x_e - x_s);
  int dy = abs(y_e - y_s);
  int dir_x = (x_s < x_e) ? 1 : -1;
  int dir_y = (y_s < y_e) ? 1 : -1;
  int err = dx - dy;

  int x = x_s;
  int y = y_s;

  while (true) {
    Draw_point(x, y, color);

    if (x == x_e && y == y_e)
      break;

    int e2 = 2 * err;

    if (e2 < dx) {
      err += dx;
      y += dir_y;
      Draw_point(x, y, color);
    }

    if (e2 > -dy) {
      err -= dy;
      x += dir_x;
    }
  }
}

void My_graphics::DrawCircleBresenham(int centerX, int centerY, int radius) {
  DrawCircleBresenham(centerX, centerY, radius, create_color(0, 255, 0, 255));
}

void My_graphics::DrawCircleBresenham(int centerX, int centerY, int radius,
                                      SDL_Color color) {
  int x = 0;
  int y = radius;
  int d = 3 - 2 * radius;

  while (y >= x) {
    Draw_point(centerX + x, centerY + y, color);
    Draw_point(centerX - x, centerY + y, color);
    Draw_point(centerX + x, centerY - y, color);
    Draw_point(centerX - x, centerY - y, color);
    Draw_point(centerX + y, centerY + x, color);
    Draw_point(centerX - y, centerY + x, color);
    Draw_point(centerX + y, centerY - x, color);
    Draw_point(centerX - y, centerY - x, color);

    if (d <= 0) {
      d = d + 4 * x + 6;
    } else {
      d = d + 4 * (x - y) + 10;
      y--;
    }
    x++;
  }
}

void My_graphics::DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2) {
  DrawTriangle(x0, y0, x1, y1, x2, y2, create_color(255, 255, 255, 100));
}

void My_graphics::DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2,
                               SDL_Color color) {

  int y_min = std::min(y0,std::min(y1,y2));
  int y_max = std::max(y0,std::max(y1,y2));
  DrawlineBresenham(x0, y0, x1, y1, create_color(0, 0, 255, 255));
  DrawlineBresenham(x1, y1, x2, y2, create_color(0, 255, 0, 255));
  DrawlineBresenham(x2, y2, x0, y0, create_color(255, 0, 0, 255));


  if (y1 == y_min) {
    std::swap(x0, x1);
    std::swap(y0, y1);
  } else if (y2 == y_min) {
    std::swap(x0, x2);
    std::swap(y0, y2);
  }
  if (y0 == y_max) {
    std::swap(x2, x0);
    std::swap(y2, y0);
  } else if (y1 == y_max) {
    std::swap(x2, x1);
    std::swap(y2, y1);
  }
  int line_x0;
  int line_x1;
  for (int i = y0; i < y1; ++i) {
    line_x0 = floor((float)x0 + ((float)i - (float)y0) * ((float)x2 - (float)x0) / ((float)y2 - (float)y0));
    line_x1 = floor((float)x0 + ((float)i - (float)y0) * ((float)x1 - (float)x0) / ((float)y1 - (float)y0));
    horisontal_line(line_x0, line_x1, i, color);
  }
  for (int i = y1; i < y2; ++i) {
    line_x0 = floor((float)x2 + ((float)i - (float)y2) * ((float)x1 - (float)x2) / ((float)y1 - (float)y2));
    line_x1 = floor((float)x2 + ((float)i - (float)y2) * ((float)x0 - (float)x2) / ((float)y0 - (float)y2));
    horisontal_line(line_x0, line_x1, i, color);
  }


}

void My_graphics::horisontal_line(int x0, int x1, int y, SDL_Color color) {
  for (int i = std::min(x0, x1); i <= std::max(x0, x1); ++i) {
    Draw_point(i, y, color);
  }
}

// void My_graphics::DrawTriangle(int x0, int y0, int x1, int y1, int x2, int
// y2,
//                                SDL_Color color) {
//   DrawlineBresenham(x0, y0, x1, y1, create_color(0, 0, 255, 255));
//   DrawlineBresenham(x1, y1, x2, y2, create_color(0, 255, 0, 255));
//   DrawlineBresenham(x2, y2, x0, y0, create_color(255, 0, 0, 255));
//   int y_min = std::min(y0,std::min(y1,y2));
//   int y_max = std::max(y0,std::max(y1,y2));
//
//   if (y1 == y_min) {
//     std::swap(x0, x1);
//     std::swap(y0, y1);
//   } else if (y2 == y_min) {
//     std::swap(x0, x2);
//     std::swap(y0, y2);
//   }
//   if (y0 == y_max) {
//     std::swap(x2, x0);
//     std::swap(y2, y0);
//   } else if (y1 == y_max) {
//     std::swap(x2, x1);
//     std::swap(y2, y1);
//   }
//   int line_x0;
//   int line_x1;
//   for (int i = y0; i < y1; i++) {
//     line_x0 = x0 + (i - y0) * (x2 - x0) / (y2 - y0);
//     line_x1 = x0 + (i - y0) * (x1 - x0) / (y1 - y0);
//     DrawlineBresenham(line_x0, i, line_x1, i, color);
//   }
//   for (int i = y1; i < y2; i++) {
//     line_x0 = x2 + (i - y2) * (x1 - x2) / (y1 - y2);
//     line_x1 = x2 + (i - y2) * (x0 - x2) / (y0 - y2);
//     DrawlineBresenham(line_x0, i, line_x1, i, color);
//   }
// }

void My_graphics::refresh_screen() {
  refresh_screen(create_color(0, 0, 0, 255));
}
void My_graphics::refresh_screen(SDL_Color color) {
  SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
  SDL_RenderClear(renderer);
}

void My_graphics::render() { SDL_RenderPresent(renderer); }

void My_graphics::DLB(int x1, int y1, int x2, int y2) {
  DLB(x1, y1, x2, y2, create_color(255, 0, 0, 255));
}

void My_graphics::DLB(int x1, int y1, int x2, int y2, SDL_Color color) {

  SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
  if (x1 == x2 && y1 == y2) {
    Draw_point(x1, y1);
    return;
  }

  int dx = abs(x2 - x1);
  int dy = abs(y2 - y1);
  bool swap = false;
  if (dy > dx) {
    swap = true;
    std::swap(x1, y1);
    std::swap(x2, y2);
    std::swap(dx, dy);
  }
  int dir_x = x2 >= x1 ? 1 : -1;
  int dir_y = y2 >= y1 ? 1 : -1;
  float t = (float)numSquares * (float)dy / (float)dx;
  float w = (float)numSquares - t;
  float d = (float)numSquares / (float)2;

  int i = dx + 1;
  if (!t) {
    while (i--) {
      lineHelp(x1, y1, swap);
      x1 += dir_x;
    }
    return;
  }
  lineHelp(x1, y1, swap);
  while (--i) {
    if (d >= w) {
      d -= w;
      y1 += dir_y;
      lineHelp(x1, y1, swap);
      x1 += dir_x;
    } else {
      d += t;
      x1 += dir_x;
    }
    lineHelp(x1, y1, swap);
  }
}
inline void My_graphics::lineHelp(int x, int y, bool swap) {

  if (swap)
    Draw_point(y, x);
  else
    Draw_point(x, y);
}
