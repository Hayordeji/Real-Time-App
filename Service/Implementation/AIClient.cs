using Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using Repository.Interface;
using Service.DTO.Message;
using Service.Interface;
using StackExchange.Redis;
using System.ClientModel;


namespace Service.Implementation
{
    public class AIClient : IAIClient
    {
        private readonly IConfiguration _config;
        private readonly IChatCompletionService _chatClient;
        private readonly IRedisCacheService _cache;
        private readonly IAIChatHistoryRepo _aiChatHistoryRepo;
        private ChatHistory? chatHistory = new();
        public AIClient(IConfiguration config, IChatCompletionService chatClient, ChatHistory history, IRedisCacheService cache, IAIChatHistoryRepo aiChatHistoryRepo)
        {
            _config = config;
            _chatClient = chatClient;
            history = new();
            _cache = cache;
            _aiChatHistoryRepo = aiChatHistoryRepo;
        }
        public async Task<string> AskAI(string question,string connectionId)
        {
            try
            {
                string? serializedChatHistory = await _cache.GetData<string>($"chatHistory_{connectionId}");
                //CHECK IF _history IS NULL AND INITIALIZE IT
                if (serializedChatHistory is null)
                {
                    ChatHistory newChatHistory = new ChatHistory();
                    //STORE IN THE DATABASE
                    var databaseChatHistory = await _aiChatHistoryRepo.CreateHistory(JsonConvert.SerializeObject(newChatHistory), connectionId);
                    chatHistory = newChatHistory;
                    //STORE IT IN CACHE
                    //await _cache.SetData<string>($"chatHistory_{connectionId}",databaseChatHistory);
                }
                else if (serializedChatHistory != null)
                {
                    var deserializedChatHistory = JsonConvert.DeserializeObject<ChatHistoryDto>(serializedChatHistory);

                    // Reconstruct ChatHistory
                    foreach (var msg in deserializedChatHistory.Messages)
                    {
                        chatHistory.AddMessage(msg.Role, msg.Content);
                    }
                    //var deserializedChatHistory = JsonConvert.DeserializeObject<ChatHistory>(serializedChatHistory);
                    //var deserializedChatHistory = JsonSerializer.Deserialize<ChatHistory>(serializedChatHistory);
                }
                // Add the user question to the history
                chatHistory.AddUserMessage(question);
                var response = await _chatClient.GetChatMessageContentAsync(chatHistory);
                if (response == null)
                {
                    Console.WriteLine("Something went wrong with the AI");
                    throw new Exception("Something went wrong with the AI");
                }
                // Add the AI response to the history
                chatHistory.Add(response);
                //string updatedSerializedChatHistory = JsonConvert.SerializeObject(chatHistory);

                var dto = new ChatHistoryDto
                {
                    Messages = chatHistory.Select(m => new ChatMessageDto
                    {
                        Role = m.Role,
                        Content = m.Content
                    }).ToList()
                };
                var json = JsonConvert.SerializeObject(dto);
                var updatedChatHistory = await _aiChatHistoryRepo.UpdateHistory(json, connectionId);
                await _cache.SetData<string>($"chatHistory_{connectionId}", json);

                return response.Content;
            
                
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
           

 






            //_history.AddUserMessage(question);

            ////STORE THE HISTROY LOCALLY
            //var localHistory2 = await _js.InvokeAsync<ChatHistory?>("localStorage.SetItem", _history, null);
            //// Call the AI service to get a response
            //var response = await _chatClient.GetChatMessageContentAsync(localHistory2);
            //if (response == null)
            //{
            //    Console.WriteLine("Something went wrong with the AI");
            //    throw new Exception("Something went wrong with the AI");
            //}
            //else
            //{
            //    var localHistory3 = await _js.InvokeAsync<ChatHistory?>("localStorage.SetItem", response, null);
            //    // Add the AI response to the history
            //    _history.Add(response);
            //    return response.Content;

            //}


        }
    } 
} 
