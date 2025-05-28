using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using Service.Interface;
using System.ClientModel;


namespace Service.Implementation
{
    public class AIClient : IAIClient
    {
        private readonly IConfiguration _config;
        private readonly IChatCompletionService _chatClient;

        public AIClient(IConfiguration config, IChatCompletionService chatClient)
        {
            _config = config;
            _chatClient = chatClient;
        }
        public async Task<string> AskAI(string question)
        {

            ChatHistory history = new ChatHistory();
            var response = await _chatClient.GetChatMessageContentAsync(question);
            history.AddUserMessage(question);
            if (response == null)
            {
                Console.WriteLine("Something went wrong with the AI");
                throw new Exception("Something went wrong with the AI");
            }
            else
            {
                history.AddAssistantMessage(response.Content);
                return response.Content;

            }


        }
    }
} 
