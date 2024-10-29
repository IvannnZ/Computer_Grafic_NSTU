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
	Clear

Clear:
	rm $(OutputFile)
