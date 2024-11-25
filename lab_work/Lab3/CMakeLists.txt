cmake_minimum_required(VERSION 3.29)
project(Lab3)

set(CMAKE_CXX_STANDARD 14)

# Подключаем директорию, чтобы видеть заголовочные файлы
include_directories(.)

# Указываем файлы проекта
add_executable(Lab3
        main.cpp
        My_grafcs.cpp
        My_grafcs.h)

# Ищем библиотеку SDL2
find_package(SDL2 REQUIRED)

# Подключаем include директории SDL2
target_include_directories(Lab3 PRIVATE ${SDL2_INCLUDE_DIRS})

# Линкуем SDL2
target_link_libraries(Lab3 PRIVATE ${SDL2_LIBRARIES})
