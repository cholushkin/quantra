@echo off
set blender_path_clean=%blender_path:"=%
set unity_import_path_clean=%unity_assets_path:"=%

REM Check if arguments are provided or help is requested
if "%1"=="" goto help
if "%1"=="-h" goto help
if "%1"=="--help" goto help

REM Run the export script with the provided arguments
"%blender_path_clean%" %1 -b -P export_fbx_object.py -- %unity_import_path_clean%\%3 %2
goto :eof

:help
echo.
echo Usage: export_fbx_object.cmd [Blender file] [Object name] [Unity asset subdirectory]
echo.
echo This script exports a specified object from a Blender file as an FBX to a Unity project.
echo.
echo Arguments:
echo   [Blender file]            - The path to the .blend file to be opened.
echo   [Object name]             - The name of the object to be exported.
echo   [Unity asset subdirectory] - The subdirectory in the Unity Assets folder where the FBX will be saved.
echo.
echo Example:
echo   export_fbx_object.cmd "C:\path\to\file.blend" "MyObject" "Models"
echo   This will export 'MyObject' from 'file.blend' to 'Assets\Models' in Unity.
echo.
goto :eof
