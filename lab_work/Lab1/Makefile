File = test.cpp
OutputFile = main.exe

CC = x86_64-w64-mingw32-g++ #myprogram.exe myprogram.cpp
CFlags = -static -mwindows

CPath = -I /home/ivannz/Programing/Computer_Grafic_NSTU/lab_work/Lab1/Graphics-main

RunProgram = wine

all: Compile Run Clear

Compile:
	$(CC) -o $(OutputFile) $(File) $(CFlags)

Run: Compile
	$(RunProgram) $(OutputFile)
	Clear

Clear:
	rm $(OutputFile)
