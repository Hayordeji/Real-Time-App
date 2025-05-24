using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IGroupRepo
    {
        Task<bool> CreateGroup(string GroupName, string? GroupDescription, string? GroupImage, string? UserId);
        Task<bool> JoinGroup(Guid Id, string UserName);

    }
}
