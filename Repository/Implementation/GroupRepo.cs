using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class GroupRepo : IGroupRepo
    {
        public Task<bool> CreateGroup(string GroupName, string? GroupDescription, string? GroupImage, string? UserId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> JoinGroup(Guid Id, string UserName)
        {
            throw new NotImplementedException();
        }
    }
}
