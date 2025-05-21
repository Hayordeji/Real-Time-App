using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IChatService
    {
        public Task ReceiveMessage(string user,  string message);
        public Task OnConnectedAsync(string method, string user, string message);
        public Task OnDisconnectedAsync(string method, string user, string message);
        public Task OnReconnectedAsync(string method, string user, string message);
        public Task AddToGroup(string groupName, string connectionId);
        public Task RemoveFromGroup(string groupName, string connectionId); 

    }
}
