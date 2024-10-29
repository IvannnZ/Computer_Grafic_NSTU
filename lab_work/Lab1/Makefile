#to install SDL2
#$sudo apt install libsdl2-dev

File = main.cpp
OutputFile = main

CC = g++
CFlags = -lSDL2

RunComand = ./

all: Compile Run Clear

Compile:
	$(CC) -o $(OutputFile) $(File) $(CFlags)

Run: Compile
	$(RunComand)$(OutputFile)


Clear:
	rm $(OutputFile)
