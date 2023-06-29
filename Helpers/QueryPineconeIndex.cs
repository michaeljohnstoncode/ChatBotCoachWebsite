using OpenAI_API.Embedding;
using OpenAI_API.Models;
using OpenAI_API;
using Pinecone.Grpc;
using Pinecone;
using ChatBotCoachWebsite.Helpers.Services;
using ChatBotCoachWebsite.Models;

namespace ChatBotCoachWebsite.Helpers
{
    public class QueryPineconeIndex
    {
        private IKeyProvider _keyProvider; 
        private IOpenAIService _openAiService;
        private BuildPineconeIndex _buildPineconeIndex;
        private uint _topIndexResults = 2; // number of returned vector texts from querying pinecone index for related context

        public QueryPineconeIndex(IKeyProvider keyProvider, BuildPineconeIndex buildPineconeIndex, IOpenAIService openAIService)
        {
            _keyProvider = keyProvider;
            _buildPineconeIndex = buildPineconeIndex;
            _openAiService = openAIService;
        }

        public async Task<MessageModel> AiCompletionResponse(List<OpenAI_API.Chat.ChatMessage> chatConversation)
        {
            OpenAIAPI openAi = _openAiService.GetOpenAI();

            //get user and userMsg from conversation. the last name/content should be the latest message, which is the user's message
            string user = chatConversation.Last().Name;
            string userMsg = chatConversation.Last().Content;

            //get full prompt+context+userMsg for chat completion
            OpenAI_API.Chat.ChatMessage fullAiPrompt = await CreateFullAiPromptAsync(user, userMsg, _topIndexResults);

            chatConversation.Add(fullAiPrompt);

            //get result of chat completion 
            OpenAI_API.Chat.ChatResult result = await openAi.Chat.CreateChatCompletionAsync(chatConversation, model: Model.ChatGPTTurbo);

            //remove full prompt from the chat conversation, it was added in to help give conversation context for the chat completion. but it is not an actual response of the conversation
            chatConversation.Remove(fullAiPrompt);

            MessageModel aiResponse = new()
            {
                User = result.Choices[0].Message.Role,
                Message = result.Choices[0].Message.Content
            };
            return aiResponse;
        }

        private async Task<OpenAI_API.Chat.ChatMessage> CreateFullAiPromptAsync(string user, string userMsg, uint topIndexResults)
        {
            List<string> contexts = await GetRelevantContextAsync(userMsg, topIndexResults);
            string context = contexts.First();

            string prompt = AiCustomPrompt();
            string fullCompletionPrompt = prompt + context + userMsg;

            OpenAI_API.Chat.ChatMessage fullAiPrompt = new()
            {
                Name = user,
                Role = OpenAI_API.Chat.ChatMessageRole.User,
                Content = fullCompletionPrompt
            };

            return fullAiPrompt;
        }

        private string AiCustomPrompt() => "Pretend that you are a coach helping teach people about competitive video games, specifically first person shooters. " +
            "The specific game you will be asked about is Overwatch. " +
            "Answer the question based on the context below: \n";

        private async Task<List<string>> GetRelevantContextAsync(string userQuestion, uint topIndexResults)
        {
            ScoredVector[]? scoredVectors = await QueryPineconeIndexAsync(userQuestion, topIndexResults);

            List<string> contexts = new();
            foreach(var vector in scoredVectors)
            {
                MetadataMap metaData = vector.Metadata;
                foreach(var data in metaData)
                {
                    var key = data.Key;
                    if(key == "name")
                    {
                        //do something with name
                    }

                    if(key == "text")
                    {
                        var value = data.Value;
                        object? text = value.Inner;
                        contexts.Add(text.ToString());
                    }
                }
            }

            return contexts;
        }

        private async Task<ScoredVector[]?> QueryPineconeIndexAsync(string userQuestion, uint topIndexResults)
        {
            //TODO: Supply an instance of OpenAI to multiple methods
            //An instance of openai is already created in BuildPineconeIndex.CreateOpenAiEmbeddingsAsync(), so there should only be one created as it doesn't need to be disposed of for different uses

            //get openAi instance
            OpenAIAPI openAi = _openAiService.GetOpenAI();

            //get pinecone index
            Index<GrpcTransport> index = await _buildPineconeIndex.GetPineconeIndexAsync();

            //TODO: omit this when providing the user question as input somewhere else (probably in the controller)
            //for now, user question is hardcoded to be used as an example question
            //userQuestion = "What can I learn about target priority?";

            //create query using openai embedding of user's question
            var embeddingQuery = await openAi.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(Model.AdaTextEmbedding, userQuestion));

            //query pinecone database for relevant context to complete ai response
            var scoredVectors = await index.Query(embeddingQuery, topK: topIndexResults, includeMetadata: true);

            return scoredVectors;
        }

    }
}
