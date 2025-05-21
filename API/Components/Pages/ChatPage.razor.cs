using Microsoft.AspNetCore.SignalR.Client;

namespace API.Components.Pages
{
    public partial class ChatPage
    {
        private string user = string.Empty;
        private string message = string.Empty;
        private List<string> messages = new List<string>();
        private HubConnection? hubConnection;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/ChatHub"))
                .WithAutomaticReconnect()
                .AddMessagePackProtocol()
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);
                InvokeAsync(StateHasChanged);
            });

            hubConnection.Reconnecting += async (error) =>
            {
                messages.Add("Attempting to reconnect...");
                await Task.Delay(500);

                try
                {
                    //START THE CONNECTION AGAIN
                    //await hubConnection.StartAsync();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Reconnect failed: " + ex.Message);

                }
                await InvokeAsync(StateHasChanged);
            };

            hubConnection.Reconnected += (connectionId) =>
            {
                messages.Add("Reconnected successfully!");
                InvokeAsync(StateHasChanged);
                return Task.CompletedTask;
            };

            hubConnection.Closed += async (error) =>
            {

            };

            //STARTS THE CONNECTION
            await hubConnection.StartAsync();

           

        }


        // This method is called when the user clicks the "Send" button
        private async Task SendMessage()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", user, message);
            }
        }

        // This method is called when the component is disposed
        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        // This method is called when the user clicks the "Start" button
        private async Task StartConnection()
        {
            if (hubConnection?.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
        }

        // / This method is called when the user clicks the "Stop" button
        private async Task StopConnection()
        {
            if (hubConnection?.State == HubConnectionState.Connected)
            {
                await hubConnection.StopAsync();
                //TEST IF ADDING THE MESSAGE AT THE TOP WILL STILL WORK
                messages.Add("Disconnected!");
            }
        }

        private async Task JoinGroup()
        {
            await hubConnection.SendAsync("JoinGroup", "Group A");
        }
        private async Task LeaveGroup()
        {
            await hubConnection.SendAsync("LeaveGroup", "Group A");
        }
    }
}
