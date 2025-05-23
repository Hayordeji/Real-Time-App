using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ITokenService
    {
        public Task<string> CreateToken(AppUser user);
    }
}
