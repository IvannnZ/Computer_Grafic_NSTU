#to install SDL2
#$sudo apt install libsdl2-dev

File = main_k.cpp
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


Prompt_data:
	echo "My_grafcs.h:"
	cat My_grafcs.h
	echo "My_grafcs.cpp:"
	cat My_grafcs.cpp
	echo "main.cpp:"
	cat main.cpp

