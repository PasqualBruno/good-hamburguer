using Microsoft.AspNetCore.SignalR;

namespace GoodHamburger.Application.Hubs;

/// <summary>
/// Hub SignalR para atualizações de pedidos em tempo real.
/// </summary>
public class OrderHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
