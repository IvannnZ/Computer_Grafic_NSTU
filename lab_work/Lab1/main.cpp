#include <SDL2/SDL.h>
#include <iostream>

void DrawGrid(SDL_Renderer *renderer, int numSquares, int squareSize);

void DrawPoint(SDL_Renderer *renderer, int x, int y, int squareSize,
               int numSquares);

int main() {

  int numSquares;     // Количество квадратов в сетке
  int squareSize;     // Размер одного квадрата
  int pointX, pointY; // Координаты точки

  std::cout << "Enter number of squares in grid: ";
  std::cin >> numSquares;
  std::cout << "Enter the size of one square: ";
  std::cin >> squareSize;
  std::cout
      << "Enter the coordinates of the point (X and Y separated by a space): ";
  std::cin >> pointX >> pointY;
  pointY--;
  pointX--;

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
  DrawGrid(renderer, numSquares, squareSize);
  DrawPoint(renderer, pointX, pointY, squareSize, numSquares);
  SDL_RenderPresent(renderer);
  int a;
  std::cin >> a;
  SDL_DestroyRenderer(renderer);
  SDL_DestroyWindow(window);
  SDL_Quit();
  return 0;
}

void DrawGrid(SDL_Renderer *renderer, int numSquares, int squareSize) {
  SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
  for (int i = 0; i <= numSquares; i++) {
    SDL_RenderDrawLine(renderer, i * squareSize, 0, i * squareSize,
                       numSquares * squareSize);
    SDL_RenderDrawLine(renderer, 0, i * squareSize, numSquares * squareSize,
                       i * squareSize);
  }
}

void DrawPoint(SDL_Renderer *renderer, int x, int y, int squareSize,
               int numSquares) {
  SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
  SDL_Rect rect = {x * squareSize,
                   numSquares * squareSize - (y + 1) * squareSize, squareSize,
                   squareSize};
  SDL_RenderFillRect(renderer, &rect);
}