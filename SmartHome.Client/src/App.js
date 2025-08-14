import React, { useState, useEffect } from 'react';
import './App.css';

function App() {
  const [state, setState] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [apiUrl, setApiUrl] = useState('/api/smarthome/state');

  useEffect(() => {
    const fetchState = async () => {
      try {
        console.log('Fetching from API...');
        const response = await fetch('/api/smarthome/state', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
        });
        
        console.log('Response status:', response.status);
        console.log('Response headers:', response.headers);
        
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status} - ${response.statusText}`);
        }
        
        const data = await response.json();
        console.log('Received data:', data);
        setState(data);
        setError(null);
      } catch (err) {
        console.error('Fetch error:', err);
        setError(`Connection error: ${err.message}. Make sure the API is running on https://localhost:7001`);
      } finally {
        setLoading(false);
      }
    };

    // Initial fetch
    fetchState();

    // Poll every second
    const interval = setInterval(fetchState, 1000);

    return () => clearInterval(interval);
  }, []);

  if (loading) {
    return (
      <div className="App">
        <div className="loading">Loading Smart Home Dashboard...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="App">
        <div className="error">Error: {error}</div>
      </div>
    );
  }

  if (!state) {
    return (
      <div className="App">
        <div className="error">No data available</div>
      </div>
    );
  }

  const formatTime = (dateTime) => {
    // Ensure dateTime is a Date object
    const date = dateTime instanceof Date ? dateTime : new Date(dateTime);
    return date.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  };

  const formatCountdown = (seconds) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  };

  const getSimulationHour = (simulationTime) => {
    return new Date(simulationTime).getHours();
  };

  const getEventTypeClass = (eventType) => {
    // Safely convert event type to string and then to lowercase
    const typeString = String(eventType).toLowerCase();
    console.log('Event type:', eventType, '-> class:', typeString);
    return typeString;
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>🏠 Smart Home Dashboard</h1>
        <p className="simulation-time">
          Simulation Time: {formatTime(state.simulationTime)}
        </p>
      </header>

      <main className="dashboard">
        <div className="status-grid">
          {/* Light Status */}
          <div className={`status-card ${state.lightOn ? 'light-on' : 'light-off'}`}>
            <h3>💡 Light Status</h3>
            <div className="status-indicator">
              {state.lightOn ? 'ON' : 'OFF'}
            </div>
            {state.lightOn && state.lightCountdown > 0 && (
              <div className="countdown">
                Turns off in: {formatCountdown(state.lightCountdown)}
              </div>
            )}
          </div>

          {/* Temperature Status */}
          <div className={`status-card ${state.heatingOn ? 'heating-on' : 'heating-off'}`}>
            <h3>🌡️ Temperature</h3>
            <div className="temperature">
              {state.temperature.toFixed(1)}°C
            </div>
            <div className="heating-status">
              Heating: {state.heatingOn ? 'ON' : 'OFF'}
            </div>
          </div>

          {/* Time Status */}
          <div className="status-card time-card">
            <h3>⏰ Time Status</h3>
            <div className="time-info">
              <div>Current: {formatTime(state.simulationTime)}</div>
              <div className={`night-indicator ${(getSimulationHour(state.simulationTime) >= 18 || getSimulationHour(state.simulationTime) < 6) ? 'night' : 'day'}`}>
                {(getSimulationHour(state.simulationTime) >= 18 || getSimulationHour(state.simulationTime) < 6) ? '🌙 Night Time' : '☀️ Day Time'}
              </div>
            </div>
          </div>
        </div>

        {/* Events Log */}
        <div className="events-section">
          <h3>📋 Recent Events</h3>
          <div className="events-list">
            {state.events.slice(-10).reverse().map((event, index) => (
              <div key={index} className={`event-item ${getEventTypeClass(event.type)}`}>
                <span className="event-time">{formatTime(event.timestamp)}</span>
                <span className="event-description">{event.description}</span>
              </div>
            ))}
          </div>
        </div>
      </main>
    </div>
  );
}

export default App;
