//https://radiofront.narod.ru/htm/prog/htm/winda/api/paint.html#3


/*

#include <windows.h>
#include "iostream"


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
*/
/*
#include "windows.h"


int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
				   PSTR lpCmdLine, int nCmdShow)
{
	return 0;
}*/


/*
#include <windows.h>
#include <stdio.h>

int main() {
    printf("Hello, Windows!\n");
    MessageBox(NULL, "Hello, Windows!", "Win32 App", MB_OK);
    return 0;
}
*/

/*
#include <windows.h>

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PWSTR pCmdLine, int nCmdShow)
{
    // Register the window class.
    const char CLASS_NAME[]  = "Sample Window Class";
    
    WNDCLASS wc = { };

    wc.lpfnWndProc   = WindowProc;
    wc.hInstance     = hInstance;
    wc.lpszClassName = CLASS_NAME;

    RegisterClass(&wc);

    // Create the window.

    HWND hwnd = CreateWindowExW(
        0,                              // Optional window styles.
        CLASS_NAME,                     // Window class
        "Learn to Program Windows",    // Window text
        WS_OVERLAPPEDWINDOW,            // Window style

        // Size and position
        CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,

        NULL,       // Parent window    
        NULL,       // Menu
        hInstance,  // Instance handle
        NULL        // Additional application data
        );

    if (hwnd == NULL)
    {
        return 0;
    }

    ShowWindow(hwnd, nCmdShow);

    // Run the message loop.

    MSG msg = { };
    while (GetMessage(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return 0;
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;

    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hwnd, &ps);

            // All painting occurs here, between BeginPaint and EndPaint.

            FillRect(hdc, &ps.rcPaint, (HBRUSH) (COLOR_WINDOW+1));

            EndPaint(hwnd, &ps);
        }
        return 0;

    }
    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}*/

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
        int  WINAPI WinMain(  HINSTANCE hInstance, HINSTANCE hPrevInst, 
        LPSTR lpszArgs, int nWinMode) {
        char szWinName[ ]="MyWindow";      //Произвольное имя класса главного окна
        char szTitle[ ]="Программа";               //Произвольный заголовок окна
        MSG msg;                    //Структура msg типа MSG для получения сообщений Windows
        WNDCLASS wc;                                  //Структура wc типа WNDCLASS для задания 
                                    //характеристик окна
        /*Заполнение структуры wc типа WNDCLASS для описания класса главного окна*/
        ZeroMemory(&wc,sizeof(wc));                             //Обнуление всех членов структуры wc
        wc.hInstance= hInstance;                                       // Дескриптор приложения
        wc.lpszClassName=szWinName;                           // Имя класса окна
        wc.lpfnWndProc=WindowFunc;                           // Определение оконной функции 
        wc.style=0;                                                            // Стиль по умолчанию
        wc.hIcon=LoadIcon(NULL,IDI_APPLICATION);           //Стандартная пиктограмма
        wc.hCursor=LoadCursor(NULL,IDC_ARROW);              //Стандартный курсор мыши
        wc.hbrBackground=GetStockBrush(WHITE_BRUSH);         // Белый фон окна
        wc.lpszMenuName=NULL;                                  // Без меню
        wc.cbClsExtra=0;                                                  // Без дополнительной информации
        wc.cbWndExtra=0;                                                // Без дополнительной информации
        /*Регистрация класса главного окна*/
        if(!RegisterClass (&wc))                                         //Если класс окна не регистрируется
        {                                    // выводим сообщение и заканчиваем выполнение программы
        MessageBox (NULL,"Окно нерегестрируется","Ошибка",MB_OK);
        return 1;}                                                           // возвращаем код ошибки
        /*Создание главного окна и отображение его на мониторе*/
        HWND hwnd = CreateWindow (                           //Вызов функции API для создания ок-на
        szWinName,                                             // имя класса главного окна
                        szTitle,                                                       // заголовок окна
                        WS_OVERLAPPEDWINDOW,               // Стиль окна 
                        100,                                                            // x-координата левого угла окна
                        100,                                                            // y-координата левого угла окна
                        500,                                                            // Ширина окна
                        100,                                                            // Высота окна
                        HWND_DESKTOP,                                  // Без родительского окна
                        NULL,                                                       // Без меню
                        hInstance,                                                  // Дескриптор приложения
                        NULL);                                                     // Без дополнительных аргументов
        ShowWindow (hwnd, SW_SHOWNORMAL);       //  Вызов функции API
                                            // для отображения окна 
        /*Организация цикла обнаружения сообщений*/
        while(GetMessage(&msg,NULL,0,0))               // Если есть сообщение, передать его
                    // нашему приложению
        DispatchMessage(&msg);                               //и вызвать оконную функцию WindowFunc 
        return 0;                                                     //После выхода из цикла вернуться в Windows
        }                                                                    //Конец функции WinMain
        
};
 
/*Оконная функция WindowFunc главного окна, вызываемая Windows и получающая в качестве параметра сообщение из очереди сообщений данного приложения */
LRESULT CALLBACK WindowFunc(HWND hwnd, UINT message,
                                WPARAM wParam, LPARAM lParam)  {
switch(message) {                                      // выбор по значению сообщения (message)
case WM_DESTROY:                           //При завершении приложения пользователем
PostQuitMessage (0);                           //вызвать функцию API завершения приложения
break; 
default:                                    // Все сообщения, не обрабатываемые данной функцией,
                                            // направляются на обработку по умолчанию 
return DefWindowProc (hwnd,message,wParam,lParam);
}                                                           //Конец оператора switch
return 0;
}
//главная функция консольного приложения
int main()
{
    
    setlocale(LC_ALL,"Russian_Russia.1251");//изменения кодировки для вывода русского языка 
    SYSTEMCRASH: //точка возврата
    std::cout<<"Введите 1 что бы нарисовать окно, \nдля выхода нажмите клавишу 2 :";
    char ch_chose = '\0';
    std::cin>>ch_chose;
    class exemple val;
    switch(ch_chose)
    {
    case '2':
            exit(0);
            break;
    case '1':
        val.WinMain( hInstance,  hPrevInst, 
         lpszArgs,  nWinMode);
            break;
    default:
        std::cout<<"Недопустимый символ попробуйте ещё! \n";
        goto SYSTEMCRASH;
    }
 
 
}