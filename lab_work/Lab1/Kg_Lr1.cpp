#include <iostream>
#include <windows.h>
#include <windowsx.h>

void DrawGrid(HDC hdc, int numSquares, int squareSize) {
    // Рисуем вертикальные линии
    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, i * squareSize, 0, NULL); // Начало вертикальной линии
        LineTo(hdc, i * squareSize, numSquares * squareSize); // Конец вертикальной линии
    }

    // Рисуем горизонтальные линии
    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, 0, i * squareSize, NULL); // Начало горизонтальной линии
        LineTo(hdc, numSquares * squareSize, i * squareSize); // Конец горизонтальной линии
    }
}

void DrawPoint(HDC hdc, int x, int y, int squareSize, int numSquares) {
    // Нарисуем точку в виде маленького круга
    HBRUSH yellowBrush = CreateSolidBrush(RGB(255, 255, 0)); // Создаем желтую кисть
    SelectBrush(hdc, yellowBrush);

    // Рассчитываем координаты для эллипса (точки) внутри квадрата
    Ellipse(hdc, x * squareSize, numSquares * squareSize - (y + 1) * squareSize, 
                 x * squareSize + squareSize, numSquares * squareSize - (y + 1) * squareSize + squareSize);

    DeleteObject(yellowBrush); // Удаляем кисть после использования
}

int main() {
    // Ввод параметров
    int numSquares; // Количество квадратов в сетке
    int squareSize; // Размер одного квадрата
    int pointX, pointY; // Координаты точки

    std::cout << "Enter number of squares in grid: ";
    std::cin >> numSquares;
    std::cout << "Enter the size of one square: ";
    std::cin >> squareSize;
    std::cout << "Enter the coordinates of the point (X and Y separated by a space): ";
    std::cin >> pointX >> pointY;

    // Получаем консольное окно и контекст устройства
    HWND hwnd = GetConsoleWindow();
    HDC hdc = GetDC(hwnd);

    // Создаем белое перо и черную кисть
    HPEN whitePen = GetStockPen(WHITE_PEN);
    HBRUSH blackBrush = GetStockBrush(BLACK_BRUSH);

    // Обновляем фон окна (заполняем черным цветом)
    SelectBrush(hdc, blackBrush);
    Rectangle(hdc, 0, 0, numSquares * squareSize, numSquares * squareSize);

    // Выбираем белое перо для рисования сетки
    SelectObject(hdc, whitePen);

    // Рисуем сетку
    DrawGrid(hdc, numSquares, squareSize);

    // Рисуем точку на заданных координатах
    DrawPoint(hdc, pointX, pointY, squareSize, numSquares);

    // Освобождаем ресурсы
    ReleaseDC(hwnd, hdc);

    // Ожидаем нажатие клавиши
    std::cin.get();
    std::cin.get();

    return 0;
}
