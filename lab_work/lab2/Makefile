#to install SDL2
#$sudo apt install libsdl2-dev

File = main.cpp  My_grafcs.cpp My_grafcs.h
OutputFile = main

CC = g++
CFlags = -lSDL2

RunComand = ./

all: Compile Run Clear

Compile:
	$(CC) $(File) -o $(OutputFile)  $(CFlags)

Run: Compile
	$(RunComand)$(OutputFile)


Clear:
	rm $(OutputFile)
