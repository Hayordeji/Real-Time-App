using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ChatService : IChatService
    {
        private readonly IMessageRepo _messageRepo;
        public ChatService(IMessageRepo messageRepo)
        {
            _messageRepo = messageRepo;
        }
        public async Task AddToGroup(string groupName, string connectionId)
        {
            await Task.CompletedTask;
        }

        public async Task RemoveFromGroup(string groupName, string connectionId)
        {
            await Task.CompletedTask;
        }

        public Task OnConnectedAsync(string method, string user, string message)
        {
            throw new NotImplementedException();
        }

        public Task OnDisconnectedAsync(string method, string user, string message)
        {
            throw new NotImplementedException();
        }

        public Task OnReconnectedAsync(string method, string user, string message)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveMessage(string user, string message)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddPersonalMessage(string senderName, string receipentName, string message)
        {
            var isAdded = await _messageRepo.CreatePersonalMessage(message, senderName, receipentName);
            if (!isAdded )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //public async Task<bool> AddGroupMessage(string user, string groupName, string message)
        //{
        //    var isAdded = await _messageRepo.CreateGroupMessage(message,user,groupName, );
        //}
    }
}
