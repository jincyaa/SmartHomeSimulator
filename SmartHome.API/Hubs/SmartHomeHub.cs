using Microsoft.AspNetCore.SignalR;

namespace SmartHome.API.Hubs;

public class SmartHomeHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"SignalR Client connected: {Context.ConnectionId}");
        Console.WriteLine($"Total connections: {Context.Items.Count}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"SignalR Client disconnected: {Context.ConnectionId}");
        if (exception != null)
        {
            Console.WriteLine($"Disconnect reason: {exception.Message}");
        }
    }
}
