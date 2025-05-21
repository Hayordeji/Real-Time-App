using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Client;

namespace API.Components.Pages
{
    public partial class GroupA : ComponentBase
    {
        public string? user { get; set; } = string.Empty;
        public bool isInitialized;
        public string? message { get; set; } = string.Empty;
        public List<string> messages = new List<string>();
        public HubConnection? _hubConnection;

        protected override async Task OnInitializedAsync()
        {
            if (isInitialized) return;
            isInitialized = true;
            //CREATE HUB CONNECTION INSTANCE
            _hubConnection = new HubConnectionBuilder()
                    .WithUrl(Navigation.ToAbsoluteUri("/GroupAHub"))
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
                await _hubConnection.SendAsync("JoinGroup", $"Group A");

        }

        private async Task SendMessageToGroup()
        {
            await _hubConnection.SendAsync("SendMessageToGroup", user,message, "Group A");
            await InvokeAsync(StateHasChanged);
        }

        private async Task LeaveGroup()
        {
            //Leave the group
            await _hubConnection.SendAsync("LeaveGroup", "Group A");

            //Stop the connection
            await _hubConnection.StopAsync();
            await InvokeAsync(StateHasChanged);

        }

      

    }
}
