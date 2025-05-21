using API.Components;
using API.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Service.Hubs;
using Service.Implementation;
using Service.Interface;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;  // Useful for debugging
    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Ping interval to keep connection alive
    options.MaximumReceiveMessageSize = 1024 * 1024;  // 1 MB limit
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // Client disconnects if no activity for 30 seconds
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);  // Time allowed to complete the 	initial handshake
    options.MaximumParallelInvocationsPerClient = 5;  // Limit parallel client invocations
}).AddMessagePackProtocol();


builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
        };
    });
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorPages();
app.UseAuthentication();
app.UseAuthorization();
//app.UseCors();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapHub<ChatHub>("/ChatHub");
app.MapHub<GroupAHub>("/GroupAHub");
app.MapHub<GroupBHub>("/GroupBHub");

app.UseResponseCompression();
app.Run();
