using Data.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public AuthService(HttpClient http, IJSRuntime js, UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _http = http;
            _js = js;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        //public async Task<bool> RegisterAsync(string username, string password)
        //{
        //    var response = await _http.PostAsJsonAsync("/api/auth/register", new { username, password });
        //    return response.IsSuccessStatusCode;
        //}

        //public async Task<bool> LoginAsync(string username, string password)
        //{
        //    var response = await _http.PostAsJsonAsync("/api/auth/login", new { username, password });
        //    if (!response.IsSuccessStatusCode) return false;

        //    var json = await response.Content.ReadAsStringAsync();
        //    var result = JsonSerializer.Deserialize<LoginResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //    if (result?.Token is not null)
        //    {
        //        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);
        //        return true;
        //    }
        //    return false;
        //}


        public async Task LogoutAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            try
            {
                var appUser = new AppUser();
                appUser.UserName = username;

                var createdUser = await _userManager.CreateAsync(appUser, password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        string token = await _tokenService.CreateToken(appUser);
                        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
                        return true;
                    }
                    else
                    {
                        throw new Exception(roleResult.Errors.ToString());
                    }
                }
                else
                {
                    throw new Exception(createdUser.Errors.ToString());
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error during registration", ex);
            }
        }

        Task<string> IAuthService.LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        private class LoginResult
        {
            public string? Token { get; set; }
        }

    }
}
