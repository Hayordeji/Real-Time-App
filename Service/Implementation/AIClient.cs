using Data.Models;
using LangChain.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using Repository.Interface;
using Service.DTO.Message;
using Service.Helpers;
using Service.Interface;
using StackExchange.Redis;
using System.ClientModel;


namespace Service.Implementation
{
    public class AIClient : IAIClient
    {
        private readonly IConfiguration _config;
        private readonly IChatCompletionService _chatClient;
        private readonly IEmbeddingService _embeddingService;
        private readonly IQdrantService _qdrantService;
        private readonly IRedisCacheService _cache;
        private readonly IAIChatHistoryRepo _aiChatHistoryRepo;
        private ChatHistory? chatHistory = new();
        public AIClient(IConfiguration config, IChatCompletionService chatClient, ChatHistory history, IRedisCacheService cache, IAIChatHistoryRepo aiChatHistoryRepo, IEmbeddingService embeddingService,
            IQdrantService qdrantService)
        {
            _config = config;
            _chatClient = chatClient;
            history = new();
            _cache = cache;
            _aiChatHistoryRepo = aiChatHistoryRepo;
            _embeddingService = embeddingService;
            _qdrantService = qdrantService;
        }
        public async Task<string> AskAI(string question,string connectionId)
        {
            try
            {
                string? serializedChatHistory = await _cache.GetData<string>($"chatHistory_{connectionId}");
                //CHECK IF _history IS NULL AND INITIALIZE IT
                if (serializedChatHistory is null)
                {
                    string filePath = "C:\\Users\\ayode\\source\\repos\\AI_Chatbot\\Service\\Helpers\\MockDataSarahChen.txt";
                    string data = File.ReadAllText(filePath);
                    string genericPrompt = await InitializeSystemPrompt(data);
                    ChatHistory newChatHistory = new ChatHistory();
                    chatHistory = newChatHistory;
                    chatHistory.AddSystemMessage(genericPrompt);
                    //STORE IN THE DATABASE
                    var databaseChatHistory = await _aiChatHistoryRepo.CreateHistory(JsonConvert.SerializeObject(chatHistory), connectionId);
                }
                else if (serializedChatHistory != null)
                {
                    var deserializedChatHistory = JsonConvert.DeserializeObject<ChatHistoryDto>(serializedChatHistory);

                    // Reconstruct ChatHistory
                    foreach (var msg in deserializedChatHistory.Messages)
                    {
                        chatHistory.AddMessage(msg.Role, msg.Content);
                    }
                    
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

        }

        public async Task<string> AskAIRemastered(string question, string connectionId)
        {
            //FETCH CACHED HISTORY IF THERE IS ANY
            string? serializedChatHistory = await _cache.GetData<string>($"chatHistory_{connectionId}");

            try
            {
                if (serializedChatHistory is null)
                {
                    //EMBED THE QUESTION
                    var embeddingResult = await _embeddingService.CreateQueryEmbedding(question, 64);

                    //FETCH SIMILAR POINTS FROM THE VECTOR STORE
                    var searchResult = await _qdrantService.SearchVector("TestCollectionSarahChen", embeddingResult, 5);

                    //GATHER THE TEXTS IN EACH POINT AND ADD IT TO THE CHATBOT SYSTEM PROMPT
                    string context = string.Join("\n", searchResult);
                    string genericPrompt = await InitializeSystemPrompt(context);

                    //INITIALIZE THE CHAT HISTORY
                    ChatHistory newChatHistory = new ChatHistory();
                    chatHistory = newChatHistory;

                    //ADD THE SYSTEM PROMPT TO THE CHAT HISTORY
                    chatHistory.AddSystemMessage(genericPrompt);
 
                }
                else if (serializedChatHistory != null)
                {
                    //DESERIALIZED CHAT HISTORY INTO AN OBJECT
                    var deserializedChatHistory = JsonConvert.DeserializeObject<ChatHistoryDto>(serializedChatHistory);

                    // Reconstruct ChatHistory
                    foreach (var msg in deserializedChatHistory.Messages)
                    {
                        chatHistory.AddMessage(msg.Role, msg.Content);
                    }

                    //EMBED THE QUESTION
                    var embeddingResult = await _embeddingService.CreateQueryEmbedding(question, 64);

                    //FETCH SIMILAR POINTS FROM THE VECTOR STORE
                    var searchResult = await _qdrantService.SearchVector("TestCollectionSarahChen", embeddingResult, 5);

                    //GATHER THE TEXTS IN EACH POINT AND ADD IT TO THE CHATBOT SYSTEM PROMPT
                    string context = string.Join("\n", searchResult);

                    //ADD THE SYSTEM PROMPT TO THE CHAT HISTORY
                    chatHistory.AddSystemMessage(context);

                }
                //ADD USER QUESTION TO THE CHAT HISTORY
                chatHistory.AddUserMessage(question);
                var response = await _chatClient.GetChatMessageContentAsync(chatHistory);
                if (response == null)
                {
                    Console.WriteLine("Something went wrong with the AI");
                    throw new Exception("Something went wrong with the AI");
                }

                // ADD THE AI RESPONSE TO THE CHAT HISTORY
                chatHistory.Add(response);

                //MAP CHAT MESSAGE AND ROLE TO A NEW OBJECT
                var dto = new ChatHistoryDto
                {
                    Messages = chatHistory.Select(m => new ChatMessageDto
                    {
                        Role = m.Role,
                        Content = m.Content
                    }).ToList()
                };
                var json = JsonConvert.SerializeObject(dto);

                //CACHE THE RESPONSE
                await _cache.SetData<string>($"chatHistory_{connectionId}", json);

                //RETURN RESPONSE
                return response.Content;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            throw new NotImplementedException();
        }

        public async Task<string> InitializeSystemPrompt(string data)
        {
            string filePath = "C:\\Users\\ayode\\source\\repos\\AI_Chatbot\\Service\\Helpers\\GenericPromptRemastered.txt";
            string fileContent =await File.ReadAllTextAsync(filePath);
            string updatedContent = fileContent.Replace("{context}", data);
             return updatedContent;
        }
    } 
} 
