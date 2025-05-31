using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Repository.Data;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class AIChatHistoryRepo : IAIChatHistoryRepo
    {
        private readonly ApplicationDbContext _context;
        public AIChatHistoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<AIChatHistory> CreateHistory(string history, string connectionId)
        {
            var newHistory = new AIChatHistory
            {
                Id = Guid.NewGuid(),
                ConnectionId = connectionId,
                ChatHistory = history
            };
            await _context.AIChatHistories.AddAsync(newHistory);
            await _context.SaveChangesAsync();
            return newHistory;
        }

        public async Task<bool> DeleteHistory(string connectionId)
        {
            var chatHistory = await _context.AIChatHistories.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
            if (chatHistory is null)
            {
                return false;
            }
            _context.AIChatHistories.Remove(chatHistory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> GetHistoryByConnectionId(string connectionId)
        {
            var history = await _context.AIChatHistories.FindAsync(connectionId);
            if (history == null)
            {
                return null;
            }
            return history.ChatHistory;
        }

        public async Task<AIChatHistory> UpdateHistory(string history, string connectionId)
        {
            var oldHistory = await _context.AIChatHistories.FirstOrDefaultAsync(h => h.ConnectionId == connectionId);
            if (oldHistory is null)
            {
                return null;
            }
            oldHistory.ChatHistory = history;
            await _context.SaveChangesAsync();
            return oldHistory;
        }
    }
}
