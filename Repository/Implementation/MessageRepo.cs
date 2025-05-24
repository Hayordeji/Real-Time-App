using Data.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Interface;
using System.Linq;

namespace Repository.Implementation
{
    public class MessageRepo : IMessageRepo
    {
        private readonly ApplicationDbContext _context;
        public MessageRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateGroupMessage(string Content, string SenderName, string GroupName, List<MessageReceipient> receipients)
        {
            var groupMessage = new Message
            {
                MessageContent = Content,
                SenderName = SenderName,
                GroupName = GroupName,
                SentAt = DateTime.UtcNow,
                IsDeleted = false,
                Receipients = receipients
            };

            await _context.Messages.AddAsync(groupMessage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreatePersonalMessage(string Content, string SenderName, string recipient)
        {
            var message = new Message
            {
                MessageContent = Content,
                SenderName = SenderName,
                ReceipientName = recipient,
                SentAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return true;
        }


        public Task<bool> DeleteMessage(Guid MessageId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Message>> GetGroupMessages(Guid GroupId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Message>> GetPersonalMessages(Guid GroupId, string? SenderName, string? ReceipientName)
        {
            var messages = await _context.Messages.Where(m => m.SenderName == SenderName && m.ReceipientName == ReceipientName).ToListAsync();
            if (messages == null)
            {
                return null;
            }
            else
            {
                return messages;
            }
        }
    }
}
