using Data.Models;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;

        }

        public async Task SendMessage(string user, string message)
        {
             _logger.LogInformation($"Message from {user} : {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToGroup(string user, string message, string groupName)
        {
            _logger.LogInformation($"Message from {user} : {message}");
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await _chatService.AddToGroup(groupName, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMessage", "System", $" A user has joined {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await _chatService.RemoveFromGroup(groupName, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $" A user has left {groupName}");
        }
        //public override async Task OnConnectedAsync()
        //{
        //    await base.OnConnectedAsync();
        //    await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveMessage","System", "A user has joined to the chat!");
        //}

        //public override async Task OnDisconnectedAsync(Exception? exception)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage","System", "A user has left the chat.");
        //    await base.OnDisconnectedAsync(exception);
        //}

        public async Task OnReconnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveMessage", "System", "A user has reconnected to the chat.");
        }
    }
}
