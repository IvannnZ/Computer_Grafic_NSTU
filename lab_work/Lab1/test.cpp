#include <graphics.h>
#include <conio.h>

int main() {
  int gd = DETECT, gm;
  char a = 'a';
  initgraph(&gd, &gm, &a);
  circle(200, 200, 100);
  getch();
  closegraph();
  return 0;
}