@echo off
echo Starting Smart Home Dashboard...
echo.

echo Starting ASP.NET Core API...
start "Smart Home API" cmd /k "cd SmartHome.API && dotnet run"

echo Waiting for API to start...
timeout /t 5 /nobreak > nul

echo Starting React App...
start "Smart Home React" cmd /k "cd SmartHome.Client && npm start"

echo.
echo Applications are starting...
echo API: https://localhost:7001
echo React App: http://localhost:3000
echo.
echo Press any key to close this window...
pause > nul
