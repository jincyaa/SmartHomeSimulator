namespace SmartHome.API.Models;

public class SmartHomeState
{
    public DateTime SimulationTime { get; set; }
    public bool LightOn { get; set; }
    public int LightCountdown { get; set; } // seconds remaining
    public double Temperature { get; set; }
    public bool HeatingOn { get; set; }
    public List<SmartHomeEvent> Events { get; set; } = new();
}

public class SmartHomeEvent
{
    public DateTime Timestamp { get; set; }
    public string Description { get; set; } = string.Empty;
    public EventType Type { get; set; }
}

public enum EventType
{
    LightOn,
    LightOff,
    MotionDetected,
    HeatingOn,
    HeatingOff,
    TemperatureChange
}
