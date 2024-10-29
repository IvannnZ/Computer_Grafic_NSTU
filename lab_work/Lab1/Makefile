File = test.cpp
OutputFile = main.exe

CC = x86_64-w64-mingw32-g++ #myprogram.exe myprogram.cpp
CFlags = -static -mwindows

RunProgram = wine

all: Compile Run Clear

Compile:
	$(CC) -o $(OutputFile) $(File) $(CFlags)

Run: Compile
	$(RunProgram) $(OutputFile)
	Clear

Clear:
	rm $(OutputFile)
