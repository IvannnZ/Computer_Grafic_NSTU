#include <SDL2/SDL.h>
#include <iostream>
#include <algorithm>

int grid = 50, size = 10;
SDL_Window *window = NULL;
SDL_Renderer *renderer = NULL;

void draw_pix(int x, int y) {
  x--; y--;
  SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
  SDL_Rect rect = {x * size, grid * size - (y + 1) * size, size, size};
  SDL_RenderFillRect(renderer, &rect);
}

inline void line_Help(int x1, int y, bool swap) {
  if (swap){draw_pix(y, x1);}
  else{draw_pix(x1, y);}
}

void draw_grid() {
  SDL_SetRenderDrawColor(renderer, 150, 150, 150, 150);
  for (int i = 0; i <= grid; i++) {
    SDL_RenderDrawLine(renderer, i * size, 0, i * size, grid * size);
    SDL_RenderDrawLine(renderer, 0, i * size, grid * size, i * size);}
}

void draw_bres(int x1, int y1, int x2, int y2) {
  if (x1 == x2 && y1 == y2) {draw_pix(x1, y1); return;}
  int dx = abs(x2 - x1), dy = abs(y2 - y1);
  bool swap = false;
  if (dy > dx) {swap = true; std::swap(x1, y1); std::swap(x2, y2); std::swap(dx, dy); }
  int sx = x2 >= x1 ? 1 : -1;
  int sy = y2 >= y1 ? 1 : -1;
  float t = grid * (float)dy / dx;
  float w = grid - t;
  float d = grid / 2.0;
  int i = dx + 1;
  if (!t) {while (i--) {line_Help(x1, y1, swap); x1 += sx;} return;}
  line_Help(x1, y1, swap);
  while (--i) {
    if (d >= w) {d -= w; y1 += sy; line_Help(x1, y1, swap); x1 += sx;}
    else {d += t; x1 += sx;}
    line_Help(x1, y1, swap);}
}

void draw_tr(int x0, int y0, int x1, int y1, int x2, int y2) {
  draw_bres(x0, y0, x1, y1); draw_bres(x1, y1, x2, y2); draw_bres(x2, y2, x0, y0);
  int y_min = std::min(y0,std::min(y1,y2));
  int y_max = std::max(y0,std::max(y1,y2));
  if (y1 == y_min) {std::swap(x0, x1); std::swap(y0, y1);}
  else if (y2 == y_min) {std::swap(x0, x2); std::swap(y0, y2);}
  if (y0 == y_max) {std::swap(x2, x0); std::swap(y2, y0);}
  else if (y1 == y_max) {std::swap(x2, x1); std::swap(y2, y1);}
  int line_x0, line_x1;
  for (int i = y0; i < y1; i++) {
    line_x0 = floor((float)x0 + (float)(i - y0) * (float)(x2 - x0) / (float)(y2 - y0));
    line_x1 = floor((float)x0 + (float)(i - y0) * (float)(x1 - x0) / (float)(y1 - y0));
    draw_bres(line_x0, i, line_x1, i);
  }
  for (int i = y1; i < y2; i++) {
    line_x0 = floor((float)x2 + (float)(i - y2) * (float)(x1 - x2) / (float)(y1 - y2));
    line_x1 = floor((float)x2 + (float)(i - y2) * (float)(x0 - x2) / (float)(y0 - y2));
    draw_bres(line_x0, i, line_x1, i);
  }
}
void refresh_screen() {
  SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
  SDL_RenderClear(renderer);}

int main() {
  int x0, y0, x1, y1, x2, y2;
  std::cout << "\nВведите координаты \nx0 y0 x1 y1 x2 y2:\n"; std::cin >> x0 >> y0 >> x1 >> y1 >> x2 >> y2;
  window = SDL_CreateWindow("SDL Window", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, grid * size + 1, grid * size + 1, SDL_WINDOW_SHOWN);
  if (!window) {
    std::cerr << "Ошибка создания окна: " << SDL_GetError() << std::endl; SDL_Quit(); return 1;}
  renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
  SDL_Event e; SDL_PollEvent(&e);
  refresh_screen(); draw_grid();
  draw_tr(x0, y0, x1, y1, x2, y2); SDL_RenderPresent(renderer);
  int a; std::cout << "Чтобы выйти нажмите любой символ\n"; std::cin >> a;
  SDL_DestroyRenderer(renderer); SDL_DestroyWindow(window); SDL_Quit();
  return 0;
}