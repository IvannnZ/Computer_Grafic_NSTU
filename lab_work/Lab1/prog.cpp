#include <windows.h>

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

void DrawPoint(HDC hdc, int x, int y, int squareSize, int numSquares) {
    Ellipse(hdc, x * squareSize, numSquares * squareSize - (y + 1) * squareSize,
            x * squareSize + squareSize, numSquares * squareSize - (y + 1) * squareSize + squareSize);
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
    switch (uMsg) {
        case WM_PAINT: {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hwnd, &ps);

            // Отрисовка сетки и точки
            int numSquares = 10;   // Пример количества квадратов
            int squareSize = 50;   // Пример размера квадрата
            int pointX = 3, pointY = 5;  // Пример координат точки
            
            DrawGrid(hdc, numSquares, squareSize);
            DrawPoint(hdc, pointX, pointY, squareSize, numSquares);

            EndPaint(hwnd, &ps);
            break;
        }
        case WM_DESTROY:
            PostQuitMessage(0);
            return 0;
    }
    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    const char CLASS_NAME[] = "GridWindowClass";

    WNDCLASS wc = {};
    wc.lpfnWndProc = WindowProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = CLASS_NAME;

    RegisterClass(&wc);

    HWND hwnd = CreateWindowEx(0, CLASS_NAME, "Grid and Point", WS_OVERLAPPEDWINDOW,
                               CW_USEDEFAULT, CW_USEDEFAULT, 800, 600, NULL, NULL, hInstance, NULL);

    if (hwnd == NULL) {
        return 0;
    }

    ShowWindow(hwnd, nCmdShow);

    // Основной цикл обработки сообщений
    MSG msg = {};
    while (GetMessage(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return 0;
}
