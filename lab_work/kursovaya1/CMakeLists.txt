cmake_minimum_required(VERSION 3.29)
project(kursovaya1)

set(CMAKE_CXX_STANDARD 14)

include_directories(.)

add_executable(kursovaya1
        main_k.cpp)

# Ищем библиотеку SDL2
find_package(SDL2 REQUIRED)

# Подключаем include директории SDL2
target_include_directories(kursovaya1 PRIVATE ${SDL2_INCLUDE_DIRS})

# Линкуем SDL2
target_link_libraries(kursovaya1 PRIVATE ${SDL2_LIBRARIES})