using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAuthService
    {
        public Task<bool> RegisterAsync(string username, string password);
        public Task<string> LoginAsync(string username, string password);
        public Task LogoutAsync();

    }
}
