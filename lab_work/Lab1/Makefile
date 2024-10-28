File = main.cpp
OutputFile = main.exe

CC = x86_64-w64-mingw32-g++ -o #myprogram.exe myprogram.cpp
CFlags = -static -mwindows

RunProgram = wine

all: Compile Run Clear

Compile:
	x86_64-w64-mingw32-g++ -o $(OutputFile) $(File) $(CFlags)

    #$(CC) -o $(OutputFile) $(File) $(CFlags)

Run: Compile
	$(RunProgram) $(OutputFile)

Clear:
	rm $(OutputFile)
