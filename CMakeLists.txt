cmake_minimum_required(VERSION 3.27)
project(PokerLogViewer NONE)

set(WPF_PROJECT "${CMAKE_SOURCE_DIR}/PokerLogViewer/PokerLogViewer.csproj")

# Генерация Visual Studio solution
add_custom_target(generate ALL
    COMMAND dotnet restore ${WPF_PROJECT}
)

# Сборка через CMake
add_custom_target(build ALL
    COMMAND dotnet build ${WPF_PROJECT} -c Release
    DEPENDS generate
)

# Опционально: запуск
add_custom_target(run
    COMMAND dotnet run --project ${WPF_PROJECT}
)