SET PATH=%~dp0;%PATH%
cd %~dp0
call npm run install
call npm run start
pause
