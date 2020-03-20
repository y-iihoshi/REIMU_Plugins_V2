@echo off

set configuration=%~1
if "%configuration%" == "" (
    exit /b 1
)

set target_framework=%~2
if "%target_framework%" == "" (
    exit /b 1
)

if not exist publish\%target_framework%\plugin (
    mkdir publish\%target_framework%\plugin
)

for %%f in (Common\bin\%configuration%\%target_framework%\*.dll) do (
    copy /b /y %%f publish\%target_framework% > NUL
)

for /d %%d in (Th*) do (
    if not "%%d" == "Th11Replay" (
        copy /b /y %%d\bin\%configuration%\%target_framework%\ReimuPlugins.%%d.dll ^
            publish\%target_framework%\plugin\ReimuPlugins.%%d.rpi > NUL
    )
)

xcopy ManualGenerator\_build\html publish\%target_framework%\doc /e /i /q
