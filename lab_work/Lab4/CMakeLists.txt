cmake_minimum_required(VERSION 3.29)
project(Lab4)

set(CMAKE_CXX_STANDARD 14)

include_directories(.)

add_executable(Lab4
        main.cpp
        My_grafcs.cpp
        My_grafcs.h)

# Ищем библиотеку SDL2
find_package(SDL2 REQUIRED)

# Подключаем include директории SDL2
target_include_directories(Lab4 PRIVATE ${SDL2_INCLUDE_DIRS})

# Линкуем SDL2
target_link_libraries(Lab4 PRIVATE ${SDL2_LIBRARIES})
