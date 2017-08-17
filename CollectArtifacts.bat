@echo off

set configuration=%~1
if "%configuration%" == "" (
    exit /b 1
)

if not exist publish\plugin (
    mkdir publish\plugin
)

for %%f in (Common\bin\%configuration%\*.dll) do (
    copy /b /y %%f publish > NUL
)

for /d %%d in (Th*) do (
    if not "%%d" == "Th11Replay" (
        copy /b /y %%d\bin\%configuration%\ReimuPlugins.%%d.dll ^
            publish\plugin\ReimuPlugins.%%d.rpi > NUL
    )
)
