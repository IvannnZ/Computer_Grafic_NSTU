#include "My_grafcs.h"
#include "iostream"
#include <SDL2/SDL.h>
#include <stack>

SDL_Color My_graphics ::create_color(int r, int g, int b, int a) {
  SDL_Color color;
  color.r = r;
  color.g = g;
  color.b = b;
  color.a = a;
  return color;
}

bool My_graphics::IsSameColor(SDL_Color c0, SDL_Color c1) {
  return c0.r == c1.r && c0.g == c1.g && c0.b == c1.b && c0.a == c1.a;
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

//  surface = SDL_CreateRGBSurfaceWithFormat(0, numSquares * squareSize + 1,
//                                           numSquares * squareSize + 1, 32,
//                                           SDL_PIXELFORMAT_RGBA32);
//  if (!surface) {
//    SDL_DestroyWindow(window);
//    SDL_DestroyRenderer(renderer);
//    SDL_Quit();
//    throw "Ошибка создания поверхности: ";
//  }
}

My_graphics::~My_graphics() {
//  SDL_FreeSurface(surface);
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

    if (x == x_e && y == y_e) {
      break;
    }

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

void My_graphics::DrawTriangleShape(int x0, int y0, int x1, int y1, int x2, int y2) {
  DrawTriangleShape(x0, y0, x1, y1, x2, y2, create_color(255, 255, 255, 100));
}

void My_graphics::DrawTriangleShape(int x0, int y0, int x1, int y1, int x2, int y2,
                               SDL_Color color) {
  DrawlineBresenham(x0, y0, x1, y1, create_color(0, 0, 255, 255));
  DrawlineBresenham(x1, y1, x2, y2, create_color(0, 255, 0, 255));
  DrawlineBresenham(x2, y2, x0, y0, create_color(255, 0, 0, 255));
}



void My_graphics::DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2) {
  DrawTriangle(x0, y0, x1, y1, x2, y2, create_color(255, 255, 255, 100));
}

void My_graphics::DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2,
                               SDL_Color color) {
  DrawlineBresenham(x0, y0, x1, y1, create_color(0, 0, 255, 255));
  DrawlineBresenham(x1, y1, x2, y2, create_color(0, 255, 0, 255));
  DrawlineBresenham(x2, y2, x0, y0, create_color(255, 0, 0, 255));

  int y_min = std::min(y0,std::min(y1,y2));
  int y_max = std::max(y0,std::max(y1,y2));

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
    line_x0 = floor(x0 + (i - y0) * (x2 - x0) / (y2 - y0));
    line_x1 = floor(x0 + (i - y0) * (x1 - x0) / (y1 - y0));
    horisontal_line(line_x0, line_x1, i, color);
  }
  for (int i = y1; i < y2; ++i) {
    line_x0 = floor(x2 + (i - y2) * (x1 - x2) / (y1 - y2));
    line_x1 = floor(x2 + (i - y2) * (x0 - x2) / (y0 - y2));
    horisontal_line(line_x0, line_x1, i, color);
  }
}

void My_graphics::horisontal_line(int x0, int x1, int y, SDL_Color color) {
  for (int i = std::min(x0, x1); i <= std::max(x0, x1); ++i) {
    Draw_point(i, y, color);
  }
}

void My_graphics::FloodFill(int x, int y, SDL_Color fillColor,
                            SDL_Color baseColor) {
  std::cout<<x<<" "<<y<<std::endl;

  if (x + 1 <= squareSize &&
      IsSameColor(baseColor, Get_Square_color(x + 1, y))) {
    Draw_point(x + 1, y, fillColor);
    render();
    FloodFill(x + 1, y, fillColor, baseColor);
  }
  if (x - 1 > 1 && IsSameColor(baseColor, Get_Square_color(x - 1, y))) {
    Draw_point(x - 1, y, fillColor);
//    render();
    FloodFill(x - 1, y, fillColor, baseColor);
  }
  if (y + 1 < squareSize &&
      IsSameColor(baseColor, Get_Square_color(x, y+1))) {
    Draw_point(x, y + 1, fillColor);
//    render();
    FloodFill(x, y + 1, fillColor, baseColor);
  }
  if (y - 1 > 1 && IsSameColor(baseColor, Get_Square_color(x , y-1))) {
    Draw_point(x, y - 1, fillColor);
//    render();
    FloodFill(x, y - 1, fillColor, baseColor);
  }

  //  SDL_Color currentColor;
  //
  //  // Функция для получения цвета пикселя
  //  auto GetPixelColor = [&](int x, int y) -> SDL_Color {
  //    Uint32 pixel;
  //    SDL_Color color;
  //    SDL_RenderReadPixels(renderer, nullptr, SDL_PIXELFORMAT_RGBA32,
  //                         surface->pixels, surface->pitch);
  //    int bpp = surface->format->BytesPerPixel;
  //    Uint8 *p = (Uint8 *)surface->pixels + (y * surface->pitch) + (x * bpp);
  //    pixel = *(Uint32 *)p;
  //    SDL_GetRGBA(pixel, surface->format, &color.r, &color.g, &color.b,
  //    &color.a); SDL_FreeSurface(surface); return color;
  //  };
  //
  //  // Функция для сравнения двух цветов
  //  auto IsSameColor = [&](SDL_Color c1, SDL_Color c2) -> bool {
  //    return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
  //  };
  //
  //  // Создаем стек для обработки пикселей
  //  std::stack<std::pair<int, int>> pixels;
  //  pixels.push({x, y});
  //
  //  while (!pixels.empty()) {
  //    auto [px, py] = pixels.top();
  //    pixels.pop();
  //
  //    currentColor = GetPixelColor(px * squareSize, py * squareSize);
  //    std::cout
  //
  //        if (!IsSameColor(currentColor, fillColor) &&
  //            !IsSameColor(currentColor, boundaryColor)) {
  //      Draw_point(px, py, fillColor);
  //
  //      pixels.push({px + 1, py});
  //      pixels.push({px - 1, py});
  //      pixels.push({px, py + 1});
  //      pixels.push({px, py - 1});
  //    }
  //  }
}

SDL_Color My_graphics::Get_Square_color(int x, int y) {
  Uint32 pixel;
  SDL_Color color;

  SDL_Surface* surface = SDL_CreateRGBSurfaceWithFormat(0, numSquares * squareSize + 1,
                                           numSquares * squareSize + 1, 32,
                                           SDL_PIXELFORMAT_RGBA32);

  SDL_RenderReadPixels(renderer, nullptr, SDL_PIXELFORMAT_RGBA32,
                       surface->pixels, surface->pitch);
  int bpp = surface->format->BytesPerPixel;
  Uint8 *p = (Uint8 *)surface->pixels +
             ((int)(y * squareSize + squareSize / 2) * surface->pitch) +
             ((int)(x * squareSize + squareSize / 2) * bpp);
  pixel = *(Uint32 *)p;
  SDL_GetRGBA(pixel, surface->format, &color.r, &color.g, &color.b, &color.a);
  SDL_FreeSurface(surface);
  return color;
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
SDL_Surface *My_graphics::GetSurface() {
  return SDL_CreateRGBSurfaceWithFormat(0, numSquares * squareSize + 1,
                                        numSquares * squareSize + 1, 32,
                                        SDL_PIXELFORMAT_RGBA32);
}