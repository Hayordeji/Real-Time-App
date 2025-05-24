using Microsoft.AspNetCore.SignalR;
using Service.Interface;

namespace API.Hubs
{
    public class GroupAHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public GroupAHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _logger = logger;
            _chatService = chatService;
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
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMessage", "System", $" A User has joined {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await _chatService.RemoveFromGroup(groupName, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $" A User has left {groupName}");
        }
    }
}
