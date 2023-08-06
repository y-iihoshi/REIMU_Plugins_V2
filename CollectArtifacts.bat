@echo off

set configuration=%~1
if "%configuration%" == "" (
    exit /b 1
)

set target_framework=%~2
if "%target_framework%" == "" (
    exit /b 1
)

set output_dir=%~3
if "%output_dir%" == "" (
    set output_dir=publish\%configuration%\%target_framework%
)

if not exist %output_dir%\plugin (
    mkdir %output_dir%\plugin
)

for %%f in (Common\bin\%configuration%\%target_framework%\*.dll) do (
    copy /b /y %%f %output_dir% > NUL
)

for /d %%d in (Th*) do (
    if not "%%d" == "Th11Replay" (
        copy /b /y %%d\bin\%configuration%\%target_framework%\ReimuPlugins.%%d.dll ^
            %output_dir%\plugin\ReimuPlugins.%%d.rpi > NUL
    )
)

xcopy ManualGenerator\_build\html %output_dir%\doc /e /i /q
