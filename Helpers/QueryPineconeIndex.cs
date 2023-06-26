using OpenAI_API.Embedding;
using OpenAI_API.Models;
using OpenAI_API;
using Pinecone.Grpc;
using Pinecone;

namespace ChatBotCoachWebsite.Helpers
{
    public class QueryPineconeIndex
    {
        private IKeyProvider _keyProvider;
        private BuildPineconeIndex _buildPineconeIndex;

        public QueryPineconeIndex(IKeyProvider keyProvider, BuildPineconeIndex buildPineconeIndex)
        {
            _keyProvider = keyProvider;
            _buildPineconeIndex = buildPineconeIndex;
        }

        public async Task CreateFullAiPromptAsync(string userQuestion, uint topIndexResults)
        {
            List<string> contexts = await GetRelevantContextAsync(userQuestion, topIndexResults);
            string context = contexts.First();

            string prompt = AiCustomPrompt();
            string fullCompletionPrompt = prompt + context + userQuestion;

            //get openAI api key
            var openAiApiKey = _keyProvider.GetKey("openaikey");
            //initialize openAi
            OpenAIAPI openAi = new OpenAIAPI(openAiApiKey);


            //TODO: Doing chat completion requires a list of chat history. 
            //      Add method for creating new list of chat messages
            //      Add method for adding to list of chat messages
            List<OpenAI_API.Chat.ChatMessage> tempChatList = new();

            OpenAI_API.Chat.ChatMessage chat = new()
            {
                Name = "student",
                Role = OpenAI_API.Chat.ChatMessageRole.User,
                Content = fullCompletionPrompt
            };

            tempChatList.Add(chat);

            var result = await openAi.Chat.CreateChatCompletionAsync(tempChatList, model: Model.ChatGPTTurbo);

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
                //TODO: Get the relevant context from metaData to be used to create the full ai prompt

            }

            return contexts;
        }

        private async Task<ScoredVector[]?> QueryPineconeIndexAsync(string userQuestion, uint topIndexResults)
        {
            //TODO: Supply an instance of OpenAI to multiple methods
            //An instance of openai is already created in BuildPineconeIndex.CreateOpenAiEmbeddingsAsync(), so there should only be one created as it doesn't need to be disposed of for different uses

            //get openAI api key
            var openAiApiKey = _keyProvider.GetKey("openaikey");
            //initialize openAi
            OpenAIAPI openAi = new OpenAIAPI(openAiApiKey);

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
