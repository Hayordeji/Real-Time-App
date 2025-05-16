using Microsoft.AspNetCore.SignalR.Client;

namespace AI_Chatbot.Components.Pages
{
    public partial class AIChatbot
    {
        private string user = string.Empty;
        private string message = string.Empty;
        private List<string> messages = new List<string>();
        private HubConnection? hubConnection;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/chatHub"))
                .WithAutomaticReconnect()
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
                     await hubConnection.StartAsync();
                     Console.WriteLine("Reconnected.");
                     await InvokeAsync(StateHasChanged);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Reconnect failed: " + ex.Message);
                    await InvokeAsync(StateHasChanged);

                }
                await InvokeAsync(StateHasChanged);
                await return Task.CompletedTask;
            };

            hubConnection.Reconnected += (connectionId) =>
            {
                messages.Add("Reconnected successfully!");
                InvokeAsync(StateHasChanged);
                return Task.CompletedTask;
            };
            
            hubConnection.Closed += async (error) =>
            {
                messages.Add("Connection lost. Attempting to reconnect...");
                
            };

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
                messages.Add("Reconnected!");
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



    }
}
