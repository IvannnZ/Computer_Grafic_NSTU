#include <SDL2/SDL.h>

class My_graphics {
private:
  int numSquares{};
  int squareSize{};
  SDL_Window *window;
  SDL_Renderer *renderer;

  void horisontal_line(int x0, int x1, int y, SDL_Color color);

  inline void lineHelp(int x, int y, bool swap);

public:
  My_graphics(int numSquares, int squareSize);

  ~My_graphics();

  static SDL_Color create_color(int r, int g, int b, int a);

  static bool IsSameColor(SDL_Color c0, SDL_Color c1);

  void refresh_screen();

  void refresh_screen(SDL_Color color);

  void render();

  void Draw_grid();

  void Draw_grid(SDL_Color color);

  void Draw_point(int x, int y);

  void Draw_point(int x, int y, SDL_Color color);

  void Draw_line_digital_differential_analyzer(int x_s, int y_s, int x_e,
                                               int y_e);

  void Draw_line_digital_differential_analyzer(int x_s, int y_s, int x_e,
                                               int y_e, SDL_Color color);

  void DrawlineBresenham(int x_s, int y_s, int x_e, int y_e);

  void DrawlineBresenham(int x_s, int y_s, int x_e, int y_e, SDL_Color color);

  void DrawCircleBresenham(int centerX, int centerY, int radius);

  void DrawCircleBresenham(int centerX, int centerY, int radius,
                           SDL_Color color);

  void DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2);

  void DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2,
                    SDL_Color color);

  void DrawTriangleShape(int x0, int y0, int x1, int y1, int x2, int y2);

  void DrawTriangleShape(int x0, int y0, int x1, int y1, int x2, int y2,
                         SDL_Color color);

  void FloodFill(int x, int y, SDL_Color fillColor, SDL_Color baseColor);

  void DLB(int x_s, int y_s, int x_e, int y_e);

  void DLB(int x_s, int y_s, int x_e, int y_e, SDL_Color color);

};