# Smart Home Dashboard

A real-time smart home monitoring system built with ASP.NET Core Web API and React, featuring automatic lighting and thermostat control simulation with real-time updates via SignalR.

## Features

- **Automatic Lighting**: Lights turn ON at night (18:00-06:00) when motion is detected, OFF after 2 minutes
- **Smart Thermostat**: Maintains 22°C ±0.5°C, drifts to 20°C when heating is off
- **Real-time Simulation**: Simulation time advances 1 minute per second, starting at 17:50
- **Event Logging**: Tracks all system events (light changes, motion detection, heating cycles)
- **Real-time Dashboard**: Beautiful React frontend with instant updates via SignalR

## Architecture

- **Backend**: ASP.NET Core 8.0 Web API with SignalR hub and CORS enabled
- **Frontend**: React 18 with modern CSS styling
- **Communication**: SignalR real-time hub + RESTful API for initial state
- **Simulation**: Background service with timer-based updates broadcasted via SignalR

## Prerequisites

- .NET 8.0 SDK
- Node.js 16+ and npm
- Visual Studio 2022 or VS Code

## Getting Started

### 1. Run the Backend API

```bash
cd SmartHome.API
dotnet restore
dotnet run
```

The API will start on `https://localhost:7001` (or similar port).

### 2. Run the React Frontend

```bash
cd SmartHome.Client
npm install
npm start
```

The React app will start on `http://localhost:3000`.

### 3. Access the Application

- **Frontend**: http://localhost:3000
- **API Documentation**: https://localhost:7001/swagger
- **API Endpoint**: GET https://localhost:7001/api/smarthome/state

## API Endpoints

### GET /api/smarthome/state

Returns the current state of the smart home system (used for initial state):

```json
{
  "simulationTime": "2024-01-01T18:30:00",
  "lightOn": true,
  "lightCountdown": 45,
  "temperature": 21.8,
  "heatingOn": true,
  "events": [
    {
      "timestamp": "2024-01-01T18:30:00",
      "description": "Motion detected at night",
      "type": "MotionDetected"
    }
  ]
}
```

### SignalR Hub: /smartHomeHub

Real-time updates are sent via SignalR hub:
- **Event**: `ReceiveSmartHomeUpdate`
- **Frequency**: Every simulated minute (every real second)
- **Data**: Complete SmartHomeState object

## Simulation Logic

### Lighting System
- **Night Detection**: 18:00 - 06:00
- **Motion Detection**: 5% chance per minute during night hours
- **Auto-off Timer**: 2 minutes (120 seconds) after motion detection
- **Day Behavior**: Lights remain off

### Thermostat System
- **Target Temperature**: 22.0°C
- **Tolerance**: ±0.5°C (21.5°C - 22.5°C)
- **Heating Behavior**: 
  - ON when temperature < 21.5°C
  - OFF when temperature ≥ 21.5°C
- **Drift Behavior**: Temperature naturally drifts toward 20.0°C when heating is off

### Time Simulation
- **Start Time**: 17:50 (5:50 PM)
- **Advancement**: 1 simulated minute per real second
- **Real-time Updates**: System state updates every second

## Project Structure

```
SmartHome/
├── SmartHome.sln                 # Solution file
├── SmartHome.API/                # ASP.NET Core Web API
│   ├── Controllers/              # API controllers
│   ├── Hubs/                     # SignalR hubs
│   ├── Models/                   # Data models
│   ├── Services/                 # Business logic services
│   ├── Program.cs                # Application entry point
│   └── SmartHome.API.csproj     # Project file
├── SmartHome.Client/             # React frontend
│   ├── public/                   # Static files
│   ├── src/                      # React source code
│   ├── package.json              # Node.js dependencies
│   └── README.md                 # Frontend documentation
└── README.md                     # This file
```

## Development

### Backend Development
- The simulation service runs as a singleton background service
- All state updates are thread-safe using locks
- Events are automatically trimmed to prevent memory issues
- SignalR hub broadcasts updates to all connected clients in real-time

### Frontend Development
- Modern React hooks (useState, useEffect)
- Responsive CSS Grid layout
- Real-time updates via SignalR connection
- Automatic reconnection handling
- Error handling and loading states

## Troubleshooting

### Common Issues

1. **CORS Errors**: Ensure the API is running and CORS is properly configured
2. **Port Conflicts**: Check if ports 7001 (API) and 3000 (React) are available
3. **Build Errors**: Ensure you have the correct .NET and Node.js versions

### Debug Mode
- Backend: Use `dotnet run --environment Development`
- Frontend: Use `npm start` (development mode with hot reload)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is open source and available under the MIT License.
