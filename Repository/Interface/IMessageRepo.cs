using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IMessageRepo
    {
        Task<List<Message>> GetGroupMessages(Guid GroupId);
        Task<List<Message>> GetPersonalMessages(Guid GroupId, string? SenderName, string? Receipient);
        Task<bool> DeleteMessage(Guid MessageId);
        Task<bool> CreatePersonalMessage(string Content, string SenderName, string RecipientName);
        Task<bool> CreateGroupMessage(string Content, string SenderName,string GroupName, List<MessageReceipient> receipients);



    }
}
