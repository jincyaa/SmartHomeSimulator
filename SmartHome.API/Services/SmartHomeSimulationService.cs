using SmartHome.API.Models;
using System.Timers;

namespace SmartHome.API.Services;

public class SmartHomeSimulationService : IDisposable
{
    private readonly System.Timers.Timer _simulationTimer;
    private readonly Random _random = new Random();
    
    private SmartHomeState _currentState;
    private DateTime _lastUpdate;
    private readonly object _lockObject = new object();

    public SmartHomeSimulationService()
    {
        // Initialize simulation at 17:50
        var today = DateTime.Today;
        _currentState = new SmartHomeState
        {
            SimulationTime = today.AddHours(17).AddMinutes(50),
            LightOn = false,
            LightCountdown = 0,
            Temperature = 22.0,
            HeatingOn = false,
            Events = new List<SmartHomeEvent>()
        };

        _lastUpdate = DateTime.UtcNow;

        // Timer runs every second, advancing simulation by 1 minute
        _simulationTimer = new System.Timers.Timer(1000);
        _simulationTimer.Elapsed += OnSimulationTick;
        _simulationTimer.Start();
    }

    private void OnSimulationTick(object? sender, ElapsedEventArgs e)
    {
        lock (_lockObject)
        {
            // Advance simulation time by 1 minute
            _currentState.SimulationTime = _currentState.SimulationTime.AddMinutes(1);
            
            // Update light countdown
            if (_currentState.LightCountdown > 0)
            {
                _currentState.LightCountdown--;
                if (_currentState.LightCountdown == 0)
                {
                    TurnLightOff();
                }
            }

            // Random motion detection at night (18:00-06:00)
            if (IsNightTime() && _random.Next(100) < 5) // 5% chance per minute
            {
                DetectMotion();
            }

            // Update temperature
            UpdateTemperature();

            _lastUpdate = DateTime.UtcNow;
        }
    }

    private bool IsNightTime()
    {
        var hour = _currentState.SimulationTime.Hour;
        return hour >= 18 || hour < 6;
    }

    private void DetectMotion()
    {
        if (!_currentState.LightOn && IsNightTime())
        {
            _currentState.LightOn = true;
            _currentState.LightCountdown = 120; // 2 minutes = 120 seconds
            
            AddEvent(EventType.MotionDetected, "Motion detected at night");
            AddEvent(EventType.LightOn, "Light turned on due to motion");
        }
    }

    private void TurnLightOff()
    {
        if (_currentState.LightOn)
        {
            _currentState.LightOn = false;
            AddEvent(EventType.LightOff, "Light turned off after countdown");
        }
    }

    private void UpdateTemperature()
    {
        var targetTemp = 22.0;
        var tolerance = 0.5;
        
        if (_currentState.HeatingOn)
        {
            // Heating is on, move towards target temperature
            if (_currentState.Temperature < targetTemp - tolerance)
            {
                _currentState.Temperature += 0.1;
                if (_currentState.Temperature >= targetTemp - tolerance)
                {
                    _currentState.HeatingOn = false;
                    AddEvent(EventType.HeatingOff, "Target temperature reached, heating off");
                }
            }
        }
        else
        {
            // Heating is off, temperature drifts towards 20°C
            if (_currentState.Temperature > 20.0)
            {
                _currentState.Temperature -= 0.05;
            }
            else if (_currentState.Temperature < 20.0)
            {
                _currentState.Temperature += 0.05;
            }

            // Check if heating should turn on
            if (_currentState.Temperature < targetTemp - tolerance)
            {
                _currentState.HeatingOn = true;
                AddEvent(EventType.HeatingOn, "Temperature below threshold, heating on");
            }
        }

        // Add temperature change event occasionally
        if (_random.Next(100) < 10) // 10% chance per minute
        {
            AddEvent(EventType.TemperatureChange, $"Temperature: {_currentState.Temperature:F1}°C");
        }
    }

    private void AddEvent(EventType type, string description)
    {
        var newEvent = new SmartHomeEvent
        {
            Timestamp = _currentState.SimulationTime,
            Type = type,
            Description = description
        };

        _currentState.Events.Add(newEvent);

        // Keep only last 50 events
        if (_currentState.Events.Count > 50)
        {
            _currentState.Events.RemoveAt(0);
        }
    }

    public SmartHomeState GetCurrentState()
    {
        lock (_lockObject)
        {
            return new SmartHomeState
            {
                SimulationTime = _currentState.SimulationTime,
                LightOn = _currentState.LightOn,
                LightCountdown = _currentState.LightCountdown,
                Temperature = _currentState.Temperature,
                HeatingOn = _currentState.HeatingOn,
                Events = _currentState.Events.ToList()
            };
        }
    }

    public void Dispose()
    {
        _simulationTimer?.Dispose();
    }
}
