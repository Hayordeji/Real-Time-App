using API.Components;
using API.Hubs;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Qdrant.Client;
using Repository.Data;
using Repository.Implementation;
using Repository.Interface;
using Service.Helpers;
using Service.Implementation;
using Service.Interface;
using Service.Mapper;
using System;
using System.Text;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageRepo, MessageRepo>();
builder.Services.AddScoped<IGroupRepo, GroupRepo>();
builder.Services.AddScoped<IAIClient, AIClient>();
builder.Services.AddSingleton<ChatHistory>();
builder.Services.AddKernel();
builder.Services.AddScoped<IRedisCacheService,RedisCacheService>();
//builder.Services.AddScoped<IVectorService, VectorService>();
builder.Services.AddScoped<IQdrantService, QdrantService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IAIChatHistoryRepo, AIChatHistoryRepo>();
builder.Services.AddSingleton<QdrantClient>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IOptions<QdrantConfig>>().Value;

    var qdrantClient = new QdrantClient("localhost", 6334);

    return qdrantClient;
});

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddOpenAIChatCompletion("gpt-4.1-nano-2025-04-14", builder.Configuration["OpenAI:APIKey"]);

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = builder.Configuration.GetConnectionString("Redis");
    option.InstanceName = "redis-1";
}); 

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    //(Add other password requirements you feel like)
}).AddEntityFrameworkStores<ApplicationDbContext>();


//JWT BEARER SETUP  
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/ChatHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

//INITIALIZE QDRANT AND EMBEDDING SERVICES
using (var scope = app.Services.CreateScope())
{
    var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();
    var qdrantService = scope.ServiceProvider.GetRequiredService<IQdrantService>();

    //CREATE COLLECTION
    //await qdrantService.CreateCollection("TestCollectionSarahChen", 64);

    //CHUNT TEXT
    //List<string> texts = await embeddingService.ChunkText();
    //CREATE EMBEDDING
    //var embedding = await embeddingService.CreateEmbeddings(texts, 64);

    //ADD VECTORS
    //await qdrantService.AddVectorsToCollection("TestCollectionSarahChen", embedding);

    //EMBED USER QUERY
    //var embeddingResponse = await embeddingService.CreateQueryEmbedding("Sarah interest and hobbies", 64);
    //SEARCH VECTORS
    //var searchResult = await qdrantService.SearchVector("TestCollectionSarahChen", embeddingResponse);

}


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
app.MapHub<ChatHub>("/ChatHub").RequireAuthorization();
//app.MapHub<GroupAHub>("/GroupAHub");
//app.MapHub<GroupBHub>("/GroupBHub");

app.UseResponseCompression();
app.Run();
