#include "My_grafcs.h"
#include <SDL2/SDL.h>
#include "iostream"

SDL_Color My_graphics ::create_color(int r, int g, int b, int a) {
  SDL_Color color;
  color.r = r;
  color.g = g;
  color.b = b;
  color.a = a;
  return color;
}

My_graphics ::My_graphics(int numSquares, int squareSize):numSquares(numSquares), squareSize(squareSize) {

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

My_graphics::~My_graphics(){
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

  // Определяем количество шагов
  int steps = std::max(dx, dy);

  // Определяем приращения для x и y
  float x_inc = dx / static_cast<float>(steps);
  float y_inc = dy / static_cast<float>(steps);

  // Начальные координаты
  float x = x_s;
  float y = y_s;

  // Рисуем пиксель на каждом шаге
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

  //  SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);

  int dx = abs(x_e - x_s);
  int dy = abs(y_e - y_s);
  int sx = (x_s < x_e) ? 1 : -1; // Направление изменения x
  int sy = (y_s < y_e) ? 1 : -1; // Направление изменения y
  int err = dx - dy;

  int x = x_s;
  int y = y_s;

  while (true) {
    // Рисуем точку на сетке
    Draw_point(x, y, color);

    // Выходим, если достигли конечной точки
    if (x == x_e && y == y_e)
      break;

    int e2 = 2 * err;

    // Двигаемся по оси x
    if (e2 > -dy) {
      err -= dy;
      x += sx;
    }

    // Двигаемся по оси y
    if (e2 < dx) {
      err += dx;
      y += sy;

      // Рисуем дополнительную точку для создания ступеньки
      // Рисуем её справа или слева от текущей точки, в зависимости от
      // направления sx
      Draw_point(x - sx, y, color);
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

  // Рисуем окружность, используя симметрию
  while (y >= x) {
    // Отображаем точки на 8 секторах
    Draw_point(centerX + x, centerY + y, color);
    Draw_point(centerX - x, centerY + y, color);
    Draw_point(centerX + x, centerY - y, color);
    Draw_point(centerX - x, centerY - y, color);
    Draw_point(centerX + y, centerY + x, color);
    Draw_point(centerX - y, centerY + x, color);
    Draw_point(centerX + y, centerY - x, color);
    Draw_point(centerX - y, centerY - x, color);

    // Обновляем параметры в зависимости от положения
    if (d <= 0) {
      d = d + 4 * x + 6;
    } else {
      d = d + 4 * (x - y) + 10;
      y--;
    }
    x++;
  }
}

void My_graphics::refresh_screen() {
  refresh_screen(create_color(0, 0, 0, 255));
}
void My_graphics::refresh_screen(SDL_Color color) {
  SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
  SDL_RenderClear(renderer);
}

void My_graphics::render() { SDL_RenderPresent(renderer); }



void My_graphics::DLB(int x1, int y1, int x2, int y2) {

  if (x1 == x2 && y1 == y2) {
    Draw_point(x1, y1);

    return;
  }
  int dx = abs(x2 - x1);
  int dy = abs(y2 - y1);
  bool swap = false;
  if (dy > dx) {
    swap = true;
    int z = x1;
    x1 = y1;
    y1 = z;
    z = x2;
    x2 = y2;
    y2 = z;
    z = dx;
    dx = dy;
    dy = z;
  }
  int sx = x2 >= x1 ? 1 : -1;
  int sy = y2 >= y1 ? 1 : -1;
  float t = numSquares * (float)dy / dx;
  float w = numSquares - t;
  float d = numSquares / 2.0;

  int i = dx + 1;
  if (!t) {
    while (i--) {
      lineHelp(x1, y1, swap);
      x1 += sx;
    }
    return;
  }
  lineHelp(x1, y1, swap);
  while (--i) {
    if (d >= w) {
      d -= w;
      y1 += sy;
      lineHelp(x1, y1, swap);
      x1 += sx;
    } else {
      d += t;
      x1 += sx;
    }
    lineHelp(x1, y1, swap);
    //    lineHelp(hDC, x1, sx, y1, size, d, swap);
  }
}
inline void My_graphics::lineHelp(int x, int y, bool swap) {

  if (swap)
    Draw_point(y, x);
  else
    Draw_point(x, y);
}
