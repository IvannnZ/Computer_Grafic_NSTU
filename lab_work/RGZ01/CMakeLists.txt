cmake_minimum_required(VERSION 3.29)
project(RGZ01)

set(CMAKE_CXX_STANDARD 20)

# Настройка путей для зависимостей
find_package(OpenGL REQUIRED)
find_package(glfw3 REQUIRED)
find_package(GLEW REQUIRED)

# Добавляем цели для glm (заголовочная библиотека)
add_library(glm INTERFACE)
target_include_directories(glm INTERFACE /usr/include/glm)

# Настройка исполняемого файла
add_executable(RGZ01 main.cpp)

# Подключаем зависимости к целевому исполняемому файлу
target_include_directories(RGZ01 PRIVATE
        ${OPENGL_INCLUDE_DIR}
        ${GLEW_INCLUDE_DIRS}
)

target_link_libraries(RGZ01
        ${OPENGL_LIBRARIES}
        glfw
        GLEW::GLEW
        glm
)

# Для Linux требуется линковать с X11 и pthread
if(UNIX AND NOT APPLE)
    target_link_libraries(RGZ01
            X11
            pthread
            dl
    )
endif()