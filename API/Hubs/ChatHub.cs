using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;
        private List<MessageReceipient> _receipients;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAIClient _aIClient;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger, UserManager<AppUser> userManager, IAIClient aIClient)
        {
            _chatService = chatService;
            _logger = logger;
            _receipients = new List<MessageReceipient>();
            _userManager = userManager;
            _aIClient = aIClient;
        }

        public async Task SendPersonalMessage(string message)
        {

            string? userName = Context.UserIdentifier;
            if (userName == null)
            {
                throw new Exception("User not found");
            }
            var connectedUser = await _userManager.FindByNameAsync(userName);
            string connectionId = Context.ConnectionId;
            
            await Clients.All.SendAsync("ReceiveMessage", connectedUser.UserName, message, Context.ConnectionId);
            _logger.LogInformation($"Message from {connectedUser.UserName} : {message}");

            //await _chatService.AddPersonalMessage(user, "ReceipientName", message);
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AskAIQuestion(string question, string chatName)
        {
            //Fetch username from JWT token
            string? username = Context.UserIdentifier;
            if (username == null) 
            {
                throw new Exception("User not found");
            }
            //check if user exists in the database
            var connectedUser = await _userManager.FindByNameAsync(username);
            //ASK AI THE QUESTION
            await Clients.Group(chatName).SendAsync("ReceiveMessage", connectedUser?.UserName, question, Context.ConnectionId);
            _logger.LogInformation($"{username} asked the chatbot a question : {question}");

            //GET RESPONSE FROM AI
            var response = await _aIClient.AskAI(question);
            _logger.LogInformation($"Chatbot response: {response}");

            //SEND THE RESPONSE TO THE GROUP
            await Clients.Group(chatName).SendAsync("ReceiveMessage", "System", response, "System Connection Id");
        }

        public async Task SendMessageToGroup(string message, string groupName)
        {
            string userName = Context.UserIdentifier;
            if (userName == null)
            {
                throw new Exception("User not found");
            }
            var connectedUser = await _userManager.FindByNameAsync(userName);
            _logger.LogInformation($"Message from {connectedUser.UserName} : {message}");
            //await _chatService
            await Clients.Group(groupName).SendAsync("ReceiveMessage", connectedUser.UserName, message, Context.ConnectionId);
            //await _chatService.AddGroupMessage(user,groupName,message);
        }

        public async Task JoinGroup(string groupName)
        {
            string userName = Context.UserIdentifier;
            if (userName == null)
            {
                throw new Exception("User not found");
            }
            var connectedUser = await _userManager.FindByNameAsync(userName);
            await _chatService.AddToGroup(groupName, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _receipients.Add(new MessageReceipient
            {
                ConnectionId = Context.ConnectionId  
            });
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMessage", "System", $"{connectedUser.UserName} has joined the chat", Context.ConnectionId);
        }

        public async Task LeaveGroup(string groupName)
        {
            await _chatService.RemoveFromGroup(groupName, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $" A user has left {groupName}", Context.ConnectionId);
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
