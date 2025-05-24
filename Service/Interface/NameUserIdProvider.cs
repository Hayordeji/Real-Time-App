using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public class NameUserIdProvider : IUserIdProvider
    {
        // This method is called by SignalR to get the user ID for a connection.
        // In this case, we are using the UserName property of the AppUser class as the user ID.
        // You can customize this method to return any unique identifier for the user.
        // For example, you could return the user's email address or a custom user ID.
        // The connection parameter contains information about the current connection.
        // You can use this information to determine the user ID for the connection.
        // For example, you could use the connection.User property to get the current user's claims.
        // In this case, we are simply returning the UserName property of the AppUser class.
        // This method is called by SignalR to get the user ID for a connection.
        public string? GetUserId(HubConnectionContext connection)
        {
            string? userName = connection.User?.FindFirst(ClaimTypes.GivenName)?.Value;
            return userName;
        }
    }
}
