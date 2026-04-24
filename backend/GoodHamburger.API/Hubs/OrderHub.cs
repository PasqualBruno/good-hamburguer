using Microsoft.AspNetCore.SignalR;

namespace GoodHamburger.API.Hubs;

/// <summary>
/// Hub SignalR para atualizações de pedidos em tempo real.
/// Grupos: "admin" (painel restaurante), "display" (tela acompanhamento), "order-{id}" (cliente específico).
/// </summary>
public class OrderHub : Hub
{
    /// <summary>
    /// Adiciona o cliente a um grupo para receber notificações específicas.
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Remove o cliente de um grupo.
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
