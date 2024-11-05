#include <SDL2/SDL.h>
#include <iostream>

void Draw_grid(SDL_Renderer *renderer, int numSquares, int squareSize);

void Draw_point(SDL_Renderer *renderer, int x, int y, int squareSize,
               int numSquares);

void Draw_line_digital_differential_analyzer(SDL_Renderer *renderer, int x_s, int y_s, int x_e, int y_e,
              int squareSize, int numSquares);

void DrawlineBresenham(SDL_Renderer *renderer, int x_s, int y_s, int x_e,
                       int y_e, int squareSize, int numSquares);

void DrawSmoothPoint(SDL_Renderer *renderer, int x, int y, int squareSize,
                     int numSquares, float alpha) {
  // Вычисляем альфа-канал для сглаживания
  int alphaChannel = static_cast<int>(255 * alpha);

  // Устанавливаем цвет сглаживания (может быть цвет линии с уменьшенной
  // прозрачностью)
  SDL_SetRenderDrawColor(renderer, 0, 255, 0, alphaChannel);

  // Преобразуем координаты и рисуем квадрат, чтобы сгладить линию
  SDL_Rect rect = {x * squareSize,
                   numSquares * squareSize - (y + 1) * squareSize, squareSize,
                   squareSize};
  SDL_RenderFillRect(renderer, &rect);
}

void DrawlineBresenhamSmooth(SDL_Renderer *renderer, int x_s, int y_s, int x_e,
                             int y_e, int squareSize, int numSquares) {
  int dx = abs(x_e - x_s);
  int dy = abs(y_e - y_s);
  int sx = (x_s < x_e) ? 1 : -1; // Направление изменения x
  int sy = (y_s < y_e) ? 1 : -1; // Направление изменения y
  int err = dx - dy;

  int x = x_s;
  int y = y_s;

  while (true) {
    // Основная точка на линии
    Draw_point(renderer, x, y, squareSize, numSquares);

    if (x == x_e && y == y_e)
      break;

    int e2 = 2 * err;

    // Сглаживание по x
    if (e2 > -dy) {
      err -= dy;
      x += sx;
      // Дополнительная точка для сглаживания
      DrawSmoothPoint(renderer, x, y + sy, squareSize, numSquares, 0.5);
    }

    // Сглаживание по y
    if (e2 < dx) {
      err += dx;
      y += sy;
      // Дополнительная точка для сглаживания
      DrawSmoothPoint(renderer, x + sx, y, squareSize, numSquares, 0.5);
    }
  }
}

int main() {

  int numSquares;     // Количество квадратов в сетке
  int squareSize;     // Размер одного квадрата
  int pointX, pointY; // Координаты точки
  int l_x_s, l_x_e, l_y_s, l_y_e;

  std::cout << "Enter number of squares in grid: ";
  std::cin >> numSquares;
  std::cout << "Enter the size of one square: ";
  std::cin >> squareSize;
  //std::cout
  //    << "Enter the coordinates of the point (X and Y separated by a space): ";
  //std::cin >> pointX >> pointY;

  std::cout << "Enter coordinate line like l_x_s, l_y_s, l_x_e, l_y_e: ";
  std::cin >> l_x_s >> l_y_s >> l_x_e >> l_y_e;

  if (SDL_Init(SDL_INIT_VIDEO) != 0) {
    std::cerr << "Ошибка инициализации SDL: " << SDL_GetError() << std::endl;
    return 1;
  }

  SDL_Window *window =
      SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED,
                       SDL_WINDOWPOS_CENTERED, numSquares*squareSize+1, numSquares*squareSize+1, SDL_WINDOW_SHOWN);
  if (!window) {
    std::cerr << "Ошибка создания окна: " << SDL_GetError() << std::endl;
    SDL_Quit();
    return 1;
  }

  SDL_Renderer *renderer =
      SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
  if (!renderer) {
    std::cerr << "Ошибка создания рендерера: " << SDL_GetError() << std::endl;
    SDL_DestroyWindow(window);
    SDL_Quit();
    return 1;
  }

  SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
  SDL_RenderClear(renderer);
  Draw_grid(renderer, numSquares, squareSize);
//  Draw_point(renderer, pointX, pointY, squareSize, numSquares);
  // Draw_line_digital_differential_analyzer(renderer, l_x_s, l_y_s, l_x_e,
  // l_y_e, squareSize, numSquares);
  //  DrawlineBresenham(renderer, l_x_s, l_y_s, l_x_e, l_y_e, squareSize,
  //                    numSquares);
  DrawlineBresenhamSmooth(renderer, l_x_s, l_y_s, l_x_e, l_y_e, squareSize,
                          numSquares);
  SDL_RenderPresent(renderer);
  int a;
  std::cin >> a;
  SDL_DestroyRenderer(renderer);
  SDL_DestroyWindow(window);
  SDL_Quit();
  return 0;
}

void Draw_grid(SDL_Renderer *renderer, int numSquares, int squareSize) {
  SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
  for (int i = 0; i <= numSquares; i++) {
    SDL_RenderDrawLine(renderer, i * squareSize, 0, i * squareSize,
                       numSquares * squareSize);
    SDL_RenderDrawLine(renderer, 0, i * squareSize, numSquares * squareSize,
                       i * squareSize);
  }
}

void Draw_point(SDL_Renderer *renderer, int x, int y, int squareSize,
               int numSquares) {
  x--;
  y--;
  SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
  SDL_Rect rect = {x * squareSize,
                   numSquares * squareSize - (y + 1) * squareSize, squareSize,
                   squareSize};
  SDL_RenderFillRect(renderer, &rect);
}

void Draw_line_digital_differential_analyzer(SDL_Renderer *renderer, int x_s, int y_s, int x_e, int y_e,
              int squareSize, int numSquares) {
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
    Draw_point(renderer, static_cast<int>(x), static_cast<int>(y), squareSize,
               numSquares);
    x += x_inc;
    y += y_inc;
  }
}

void DrawlineBresenham(SDL_Renderer *renderer, int x_s, int y_s, int x_e,
                       int y_e, int squareSize, int numSquares) {
  int dx = abs(x_e - x_s);
  int dy = abs(y_e - y_s);
  int sx = (x_s < x_e) ? 1 : -1; // Направление изменения x
  int sy = (y_s < y_e) ? 1 : -1; // Направление изменения y
  int err = dx - dy;

  int x = x_s;
  int y = y_s;

  while (true) {
    // Рисуем точку на сетке
    Draw_point(renderer, x, y, squareSize, numSquares);

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
      Draw_point(renderer, x - sx, y, squareSize, numSquares);
    }
  }
}
