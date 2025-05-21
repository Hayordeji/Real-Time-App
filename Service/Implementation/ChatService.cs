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
    }
}
