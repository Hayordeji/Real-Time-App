using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Client;

namespace API.Components.Pages
{
    public partial class GroupB : ComponentBase
    {
        public string? user { get; set; } = string.Empty;
        public string? message { get; set; } = string.Empty;
        public List<string> messages = new List<string>();
        public HubConnection? _hubConnection;

        protected override async Task OnInitializedAsync()
        {
            
            //CREATE HUB CONNECTION INSTANCE
            _hubConnection = new HubConnectionBuilder()
                    .WithUrl(Navigation.ToAbsoluteUri("/ChatHub"))
                    .WithAutomaticReconnect()
                    .AddMessagePackProtocol()
                    .Build();
            //START THE CONNECTION
            await _hubConnection.StartAsync();

            //LISTEN FOR MESSAGES
            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);
                InvokeAsync(StateHasChanged);
            });

            //CUSTOM METHOD TO JOIN A GROUP AND SEND MESSAGE ABOUT JOINING
            await _hubConnection.SendAsync("JoinGroup", $"Group B");

        }

        private async Task SendMessageToGroup()
        {
            await _hubConnection.SendAsync("SendMessageToGroup", user, message, "Group B");
            await InvokeAsync(StateHasChanged);
        }

        private async Task LeaveGroup()
        {
            //Leave the group
            await _hubConnection.SendAsync("LeaveGroup", "Group B");

            //Stop the connection
            await _hubConnection.StopAsync();
            await InvokeAsync(StateHasChanged);

        }



    }
}
