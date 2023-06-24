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

        private string AiCustomPrompt() => "Answer the question based on the context below";

        private void CreateFullAiPrompt()
        {

        }

        public async Task GetRelevantContextAsync(string userQuestion, uint topIndexResults)
        {
            ScoredVector[]? scoredVectors = await QueryPineconeIndexAsync(userQuestion, topIndexResults);

            foreach(var vector in scoredVectors)
            {
                MetadataMap metaData = vector.Metadata;
                
                //TODO: Get the relevant context from metaData to be used to create the full ai prompt
            }
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
