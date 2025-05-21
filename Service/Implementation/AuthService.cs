using Microsoft.JSInterop;
using Service.Interface;
using System.Net.Http.Json;
using System.Text.Json;

namespace Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private const string TokenKey = "jwt_token";

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var response = await _http.PostAsJsonAsync("/api/auth/register", new { username, password });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _http.PostAsJsonAsync("/api/auth/login", new { username, password });
            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Token is not null)
            {

                await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
                return true;
            }
            return false;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);
        }

        public async Task LogoutAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }

        private class LoginResult
        {
            public string? Token { get; set; }
        }

    }
}
