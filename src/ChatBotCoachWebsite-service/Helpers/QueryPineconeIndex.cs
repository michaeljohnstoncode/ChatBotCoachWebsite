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
        private GetPineconeIndex _buildPineconeIndex;
        private uint _topIndexResults = 2; // number of vector results from querying pinecone index for related context

        public QueryPineconeIndex(IKeyProvider keyProvider, GetPineconeIndex buildPineconeIndex, IOpenAIService openAIService)
        {
            _keyProvider = keyProvider;
            _buildPineconeIndex = buildPineconeIndex;
            _openAiService = openAIService;
        }

        public async Task<UserMessage> AiCompletionResponse(List<OpenAI_API.Chat.ChatMessage> chatConversation)
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

            UserMessage aiResponse = new()
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
            string fullCompletionPrompt = prompt + context + "\nUser message: " + userMsg;

            OpenAI_API.Chat.ChatMessage fullAiPrompt = new()
            {
                Name = user,
                Role = OpenAI_API.Chat.ChatMessageRole.User,
                Content = fullCompletionPrompt
            };

            return fullAiPrompt;
        }

        private string AiCustomPrompt() => "Act as if you are a coach that has experience and enjoys helping and teaching others. " +
            "You are knowledgeable, confident, and you are a teacher. The information that you source from is yours that you have experienced and teach from." +
            " Start by asking for context about the user. Ask what their goals are for Overwatch (fun vs. winning vs. improving) and why they are seeking to be coached. " +
            "Context like their competitive rank, how many hours they have in game and what gamemode, what characters they main or like to play," +
            " what role they tend to play, etc are important to get context for coaching. " +
            "The specific game you will be asked about is Overwatch. " +
            "Answer the question/chat message based on the context below: \n";

        private async Task<List<string>> GetRelevantContextAsync(string userQuestion, uint topIndexResults)
        {
            //query the index for relevant vectors
            ScoredVector[]? scoredVectors = await QueryPineconeIndexAsync(userQuestion, topIndexResults);

            //convert the relevant vector(s) into string text
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
                        //convert the text in MetadataValue object to a string
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
            //get openAi instance
            OpenAIAPI openAi = _openAiService.GetOpenAI();

            //get pinecone index
            Index<GrpcTransport> index = await _buildPineconeIndex.GetPineconeIndexAsync();

            //create query using openai embedding of user's question
            var embeddingQuery = await openAi.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(Model.AdaTextEmbedding, userQuestion));

            //query pinecone database for relevant context to complete ai response
            var scoredVectors = await index.Query(embeddingQuery, topK: topIndexResults, includeMetadata: true);

            return scoredVectors;
        }

    }
}
