@echo off
cls
"C:\Program Files (x86)\Mono-3.2.3\bin\xbuild.bat" /p:Configuration=Release /t:Clean;Build src\Nerve.sln