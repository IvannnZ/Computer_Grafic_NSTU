#include <SDL2/SDL.h>
#include <iostream>

void DrawGrid(SDL_Renderer *renderer, int numSquares, int squareSize) {
  // Рисуем вертикальные линии
  SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255); // Красный цвет
  for (int i = 0; i <= numSquares; i++) {
    SDL_RenderDrawLine(renderer, i * squareSize, 0, i * squareSize,
                       numSquares * squareSize);
    SDL_RenderDrawLine(renderer, 0, i * squareSize, numSquares * squareSize,
                       i * squareSize);
  }
  SDL_RenderPresent(renderer);
}


void DrawPoint(SDL_Renderer* renderer, int x, int y, int squareSize, int numSquares) {
  SDL_Rect rect = {x * squareSize, numSquares * squareSize - (y + 1) * squareSize, squareSize, squareSize}; // Прямоугольник (x, y, ширина, высота)
  SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255); // Зеленый цвет
  SDL_RenderFillRect(renderer, &rect);

  // Отображение изменений
  SDL_RenderPresent(renderer);

}


int main() {
  if (SDL_Init(SDL_INIT_VIDEO) != 0) {
    std::cerr << "Ошибка инициализации SDL: " << SDL_GetError() << std::endl;
    return 1;
  }

  SDL_Window *window =
      SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED,
                       SDL_WINDOWPOS_CENTERED, 800, 600, SDL_WINDOW_SHOWN);
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

  DrawGrid(renderer, 10, 30);
  int a;
  std::cin>>a;
  // Рисование прямоугольника

  SDL_DestroyRenderer(renderer);
  SDL_DestroyWindow(window);
  SDL_Quit();
  return 0;
}
