// Kg_Lr1.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//




#include <iostream>
#include <windows.h>                     //Два файла с определениями, макросами
#include <windowsx.h>                  //и прототипами функций Windows
//глобальные переменные для рисования окна
HINSTANCE hInstance; HINSTANCE hPrevInst;
LPSTR lpszArgs; int nWinMode;
/*Прототип используемой в программе оконной функции */
LRESULT CALLBACK WindowFunc(HWND, UINT, WPARAM, LPARAM);
/*Произвольный класс*/
class exemple
{
public:
    /*Главная функция приложения WinMain*/
    int  WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInst,
        LPSTR lpszArgs, int nWinMode) {
        MSG msg;                    //Структура msg типа MSG для получения сообщений Windows
        WNDCLASS wc;                                  //Структура wc типа WNDCLASS для задания 
        //характеристик окна
/*Заполнение структуры wc типа WNDCLASS для описания класса главного окна*/
        ZeroMemory(&wc, sizeof(wc));                             //Обнуление всех членов структуры wc
        wc.hInstance = hInstance;                                       // Дескриптор приложения
        wc.lpszClassName = L"aaaa";                           // Имя класса окна
        wc.lpfnWndProc = WindowFunc;                           // Определение оконной функции 
        wc.style = 0;                                                            // Стиль по умолчанию
        wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);           //Стандартная пиктограмма
        wc.hCursor = LoadCursor(NULL, IDC_ARROW);              //Стандартный курсор мыши
        wc.hbrBackground = GetStockBrush(WHITE_BRUSH);         // Белый фон окна
        wc.lpszMenuName = NULL;                                  // Без меню
        wc.cbClsExtra = 0;                                                  // Без дополнительной информации
        wc.cbWndExtra = 0;                                                // Без дополнительной информации
        /*Регистрация класса главного окна*/
        if (!RegisterClass(&wc))                                         //Если класс окна не регистрируется
        {                                   
            return 1;
        }                                                           // возвращаем код ошибки
/*Создание главного окна и отображение его на мониторе*/
        ShowWindow(CreateWindow(L"dsdsds", L"prog", WS_OVERLAPPEDWINDOW, 100, 100, 500, 100, HWND_DESKTOP, NULL, hInstance, NULL), SW_SHOWNORMAL);       //  Вызов функции API
        // для отображения окна 
/*Организация цикла обнаружения сообщений*/
        while (GetMessage(&msg, NULL, 0, 0))               // Если есть сообщение, передать его
            // нашему приложению
            DispatchMessage(&msg);                               //и вызвать оконную функцию WindowFunc 
        return 0;                                                     //После выхода из цикла вернуться в Windows
    }                                                                    //Конец функции WinMain

};

/*Оконная функция WindowFunc главного окна, вызываемая Windows и получающая в качестве параметра сообщение из очереди сообщений данного приложения */
LRESULT CALLBACK WindowFunc(HWND hwnd, UINT message,
    WPARAM wParam, LPARAM lParam) {
    switch (message) {                                      // выбор по значению сообщения (message)
    case WM_DESTROY:                           //При завершении приложения пользователем
        PostQuitMessage(0);                           //вызвать функцию API завершения приложения
        break;
    default:                                    // Все сообщения, не обрабатываемые данной функцией,
        // направляются на обработку по умолчанию 
        return DefWindowProc(hwnd, message, wParam, lParam);
    }                                                           //Конец оператора switch
    return 0;
}
//главная функция консольного приложения
int main()
{

    //setlocale(LC_ALL, "Russian_Russia.1251");//изменения кодировки для вывода русского языка 
//SYSTEMCRASH: //точка возврата
    class exemple val;
        val.WinMain(hInstance, hPrevInst,
            lpszArgs, nWinMode);
    


}


/*
#include <iostream>
#include <windows.h>
#include <windowsx.h>

void DrawGrid(HDC hdc, int numSquares, int squareSize) {
    int x = 0;
    int y = 0;

    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, x + i * squareSize, y, NULL);
        LineTo(hdc, x + i * squareSize, y + numSquares * squareSize);
        //Sleep(10);
    }

    for (int i = 0; i <= numSquares; i++) {
        MoveToEx(hdc, x, y + i * squareSize, NULL); 
        LineTo(hdc, x + numSquares * squareSize, y + i * squareSize);
        //Sleep(10);
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
//}

// Запуск программы: CTRL+F5 или меню "Отладка" > "Запуск без отладки"
// Отладка программы: F5 или меню "Отладка" > "Запустить отладку"

// Советы по началу работы 
//   1. В окне обозревателя решений можно добавлять файлы и управлять ими.
//   2. В окне Team Explorer можно подключиться к системе управления версиями.
//   3. В окне "Выходные данные" можно просматривать выходные данные сборки и другие сообщения.
//   4. В окне "Список ошибок" можно просматривать ошибки.
//   5. Последовательно выберите пункты меню "Проект" > "Добавить новый элемент", чтобы создать файлы кода, или "Проект" > "Добавить существующий элемент", чтобы добавить в проект существующие файлы кода.
//   6. Чтобы снова открыть этот проект позже, выберите пункты меню "Файл" > "Открыть" > "Проект" и выберите SLN-файл.
