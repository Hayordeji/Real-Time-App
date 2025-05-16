using Data.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Hubs
{
    public class ChatHub : Hub<IChatService>
    {
        //public async Task JoinedChat(UserConnection connection)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", "admin", $"{connection.Username} has joined");
        //}

        //public async Task JoinSpecificChat(UserConnection connection)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);
        //    await Clients.Group(connection.ChatRoom).SendAsync("ReceiveMessage", "admin", $"{connection.Username} has joined");
        //}
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
        public async Task SendMessage(string user, string message)
        {
             _logger.LogInformation($"Message from {user} : {message}");
            await Clients.All.ReceiveMessage(user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.All.ReceiveMessage("System", "Welcome to the chat!");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.ReceiveMessage("System", "A user has left the chat.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
