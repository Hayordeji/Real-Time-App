using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IAIChatHistoryRepo
    {
        Task<AIChatHistory> CreateHistory(string history, string connectionId);
        Task<string?> GetHistoryByConnectionId(string connectionId);
        Task<AIChatHistory> UpdateHistory(string history, string connectionId);
        Task<bool> DeleteHistory(string connectionId);

    }
}
