// Kg_Lr1.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//

#include <iostream>
#include <windows.h>
#include <windowsx.h>

void DrawGrid(HDC hdc, int numSquares, int squareSize) {
    int x = 0;
    int y = 0;

    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, x + i * squareSize, y, NULL);
        LineTo(hdc, x + i * squareSize, y + numSquares * squareSize);
        Sleep(10);
    }

    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, x, y + i * squareSize, NULL); 
        LineTo(hdc, x + numSquares * squareSize, y + i * squareSize);
        Sleep(10);
    }
}

void DrawPoint(HDC hdc, int x, int y, int squareSize, int num_Squares) {
    Ellipse(hdc, x * squareSize,num_Squares * squareSize - (y+1)  * squareSize, x* squareSize + squareSize, num_Squares * squareSize - (y+1)* squareSize + squareSize);
}

int main()
{
    int numSquares;
    int squareSize;
    int pointX, pointY;
    int pointSize;

    std::cout << "Enter Num squares in grid: ";
    std::cin >> numSquares;
    std::cout << "Enter the size of one square: ";
    std::cin >> squareSize;

    std::cout << "Enter the coordinates of the point (X and Y separated by a space): ";
    std::cin >> pointX >> pointY;
    //std::cout << "Enter scale of dot: ";
    //std::cin >> pointSize;
    
    system("cls");

    HWND hwnd = GetConsoleWindow();
    HDC hdc = GetDC(hwnd);

    HPEN whitePen = GetStockPen(WHITE_PEN);
    HBRUSH blackBrush = GetStockBrush(BLACK_BRUSH);

    SelectObject(hdc, whitePen);
    DrawGrid(hdc, numSquares, squareSize);

    SelectObject(hdc, blackBrush);
    DrawPoint(hdc, pointX, pointY, squareSize, numSquares);

    ReleaseDC(hwnd, hdc);

    int i;
    std::cin >> i;

    return 0;



    /*
    HWND hwnd = GetConsoleWindow();
    HDC hdc = GetDC(hwnd);
    
    HBRUSH blackBrush = GetStockBrush(BLACK_BRUSH);

    SelectBrush(hdc, blackBrush);
    FloodFill(hdc, 0, 0, RGB(0, 0, 1));

    HPEN whitePen = GetStockPen(WHITE_PEN);
    SelectPen(hdc, whitePen);

    MoveToEx(hdc, 50, 100, NULL);
    LineTo(hdc, 120, 90);

    int i;
    std::cin >> i;

    DeleteObject(whitePen);
    ReleaseDC(hwnd, hdc);

    return 0;*/
}

// Запуск программы: CTRL+F5 или меню "Отладка" > "Запуск без отладки"
// Отладка программы: F5 или меню "Отладка" > "Запустить отладку"

// Советы по началу работы 
//   1. В окне обозревателя решений можно добавлять файлы и управлять ими.
//   2. В окне Team Explorer можно подключиться к системе управления версиями.
//   3. В окне "Выходные данные" можно просматривать выходные данные сборки и другие сообщения.
//   4. В окне "Список ошибок" можно просматривать ошибки.
//   5. Последовательно выберите пункты меню "Проект" > "Добавить новый элемент", чтобы создать файлы кода, или "Проект" > "Добавить существующий элемент", чтобы добавить в проект существующие файлы кода.
//   6. Чтобы снова открыть этот проект позже, выберите пункты меню "Файл" > "Открыть" > "Проект" и выберите SLN-файл.
