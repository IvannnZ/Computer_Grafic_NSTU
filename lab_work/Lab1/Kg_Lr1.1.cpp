#include <iostream>
#include <windows.h>
#include <windowsx.h>

void DrawGrid(HDC hdc, int numSquares, int squareSize) {
    int x = 0;
    int y = 0;

    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, x + i * squareSize, y, NULL);
        LineTo(hdc, x + i * squareSize, y + numSquares * squareSize);
    }

    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, x, y + i * squareSize, NULL);
        LineTo(hdc, x + numSquares * squareSize, y + i * squareSize);
    }
}

void DrawPoint(HDC hdc, int x, int y, int squareSize, int numSquares) {
    Ellipse(hdc, x * squareSize, numSquares * squareSize - (y + 1) * squareSize,
            x * squareSize + squareSize, numSquares * squareSize - (y + 1) * squareSize + squareSize);
}

int main() {
    int numSquares;
    int squareSize;
    int pointX, pointY;

    std::cout << "Enter Num squares in grid: ";
    std::cin >> numSquares;
    std::cout << "Enter the size of one square: ";
    std::cin >> squareSize;
    std::cout << "Enter the coordinates of the point (X and Y separated by a space): ";
    std::cin >> pointX >> pointY;

    // Получаем дескриптор окна консоли
    HWND hwnd = GetConsoleWindow();
    if (hwnd == NULL) {
        std::cerr << "Error: Cannot get console window handle.\n";
        return 52;
    }

    HDC hdc = GetDC(hwnd);  // Получаем контекст устройства для консольного окна

    if (hdc == NULL) {
        std::cerr << "Error: Cannot get device context.\n";
        return 42;
    }

    HPEN whitePen = CreatePen(PS_SOLID, 1, RGB(255, 255, 255));
    HBRUSH blackBrush = CreateSolidBrush(RGB(0, 0, 0));

    SelectObject(hdc, whitePen);
    DrawGrid(hdc, numSquares, squareSize);

    SelectObject(hdc, blackBrush);
    DrawPoint(hdc, pointX, pointY, squareSize, numSquares);

    // Освобождаем ресурсы
    DeleteObject(whitePen);
    DeleteObject(blackBrush);
    ReleaseDC(hwnd, hdc);

    std::cout << "Press any key to exit..." << std::endl;
    std::cin.get();  // Ожидание ввода перед завершением

    return 0;
}
