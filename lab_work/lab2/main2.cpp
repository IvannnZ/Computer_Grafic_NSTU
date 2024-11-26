#include <SDL2/SDL.h>
#include <iostream>

int numSquares; // Количество квадратов в сетке
int squareSize; // Размер одного квадрата
SDL_Window *window = NULL;
SDL_Renderer *renderer = NULL;

inline void lineHelp(int x1, int y, bool swap);

void DrawLineBresenham(int x1, int y1, int x2, int y2);

void Draw_grid();

void Draw_point(int x, int y);

void Draw_line_digital_differential_analyzer(int x_s, int y_s, int x_e,
                                             int y_e);

void DrawlineBresenham(int x_s, int y_s, int x_e, int y_e);

void DrawCircleBresenham(int centerX, int centerY, int radius);

void refresh_screen() {
  SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
  SDL_RenderClear(renderer);
}

int main() {


  std::cout << "Enter number of squares in grid: ";
  std::cin >> numSquares;
  std::cout << "Enter the size of one square: ";
  std::cin >> squareSize;

  if (SDL_Init(SDL_INIT_VIDEO) != 0) {
    std::cerr << "Ошибка инициализации SDL: " << SDL_GetError() << std::endl;
    return 1;
  }

  window = SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED,
                            SDL_WINDOWPOS_CENTERED, numSquares * squareSize + 1,
                            numSquares * squareSize + 1, SDL_WINDOW_SHOWN);
  if (!window) {
    std::cerr << "Ошибка создания окна: " << SDL_GetError() << std::endl;
    SDL_Quit();
    return 1;
  }

  renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
  if (!renderer) {
    std::cerr << "Ошибка создания рендерера: " << SDL_GetError() << std::endl;
    SDL_DestroyWindow(window);
    SDL_Quit();
    return 1;
  }
  std::cout << "1-dot 2-line 3-circle: ";
  int chouse;
  std::cin >> chouse;
  refresh_screen();
  Draw_grid();

  switch (chouse) {
  case 1:
    int pointX, pointY; // Координаты точки
    std::cout << "Enter the coordinates of the point (X and Y separated by a "
                 "space): ";
    std::cin >> pointX >> pointY;
    Draw_point(pointX, pointY);
    break;
  case 2:

    std::cout << "1 - Draw_line_digital_differential_analyzer\n2 - "
                 "DrawlineBresenham: ";
    std::cin >> chouse;
    int l_x_s, l_x_e, l_y_s, l_y_e;
    std::cout << "Enter coordinate line like l_x_s, l_y_s, l_x_e, l_y_e: ";
    std::cin >> l_x_s >> l_y_s >> l_x_e >> l_y_e;

    switch (chouse) {
    case 1:
      Draw_line_digital_differential_analyzer(l_x_s, l_y_s, l_x_e, l_y_e);
      break;

    case 2:
      DrawLineBresenham(l_x_s, l_y_s, l_x_e, l_y_e);
      break;
    }
    break;

  case 3:
    std::cout << "Enter center circle x, y, and radius: ";
    int centerX, centerY, radius;
    std::cin >> centerX >> centerY >> radius;
    DrawCircleBresenham(centerX, centerY, radius);
    break;
  }
  SDL_RenderPresent(renderer);
  int a;
  std::cin >> a;
  SDL_DestroyRenderer(renderer);
  SDL_DestroyWindow(window);
  SDL_Quit();
  return 0;
}

void Draw_grid() {
  SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
  for (int i = 0; i <= numSquares; i++) {
    SDL_RenderDrawLine(renderer, i * squareSize, 0, i * squareSize,
                       numSquares * squareSize);
    SDL_RenderDrawLine(renderer, 0, i * squareSize, numSquares * squareSize,
                       i * squareSize);
  }
}

void Draw_point(int x, int y) {
  x--;
  y--;
  SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
  SDL_Rect rect = {x * squareSize,
                   numSquares * squareSize - (y + 1) * squareSize, squareSize,
                   squareSize};
  SDL_RenderFillRect(renderer, &rect);
}

void Draw_line_digital_differential_analyzer(int x_s, int y_s, int x_e,
                                             int y_e) {
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
    Draw_point(static_cast<int>(x), static_cast<int>(y));
    x += x_inc;
    y += y_inc;
  }
}

void DrawCircleBresenham(int centerX, int centerY, int radius) {
  int x = 0;
  int y = radius;
  int d = 3 - 2 * radius;

  // Рисуем окружность, используя симметрию
  while (y >= x) {
    // Отображаем точки на 8 секторах
    Draw_point(centerX + x, centerY + y);
    Draw_point(centerX - x, centerY + y);
    Draw_point(centerX + x, centerY - y);
    Draw_point(centerX - x, centerY - y);
    Draw_point(centerX + y, centerY + x);
    Draw_point(centerX - y, centerY + x);
    Draw_point(centerX + y, centerY - x);
    Draw_point(centerX - y, centerY - x);

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

void DrawLineBresenham(int x1, int y1, int x2, int y2) {

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
  }
}
inline void lineHelp(int x1, int y, bool swap) {

  if (swap)
    Draw_point(y, x1);
  else
    Draw_point(x1, y);
}
