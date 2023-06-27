using ChatBotCoachWebsite.Helpers.Services;
using OpenAI_API;
using OpenAI_API.Embedding;
using OpenAI_API.Models;
using Pinecone;
using Pinecone.Grpc;


namespace ChatBotCoachWebsite.Helpers
{
    public class VectorDataModel
    {
        public string FileName { get; set; }
        public string FileData { get; set; }
        public EmbeddingResult EmbeddingResult { get; set; }
    }

    public class BuildPineconeIndex
    {

        private ICustomDataProvider _customDataProvider;
        private IKeyProvider _keyProvider;
        private IOpenAIService _openAiService;

        public BuildPineconeIndex(ICustomDataProvider customDataProvider, IKeyProvider keyProvider, IOpenAIService openAIService)
        {
            _customDataProvider = customDataProvider;
            _keyProvider = keyProvider;
            _openAiService = openAIService;
        }

        public async Task UpsertPineconeIndexAsync()
        {
            //get vectors for index
            Vector[] vectors = await CreateVectorsAsync();

            //get pinecone index
            Index<GrpcTransport> index = await GetPineconeIndexAsync();

            //configure index
            //Configuring indexes is NOT supported in free tier.
            //calling ConfigureIndex is not necessary unless explicitly required. creation of index comes with default configuration
            //await pinecone.ConfigureIndex(indexName, replicas: 1, podType: "p1");

            //upsert vectors into index
            await index.Upsert(vectors);
        }

        public async Task<Index<GrpcTransport>> GetPineconeIndexAsync()
        {
            //get instance of pinecone client
            var pinecone = InitializePineCone();

            //get list of pinecone indexes
            var indexes = await pinecone.ListIndexes();

            //index name (only have 1 index for now)
            var indexName = "compcoachindex";

            //create a new pinecone index incase it doesn't exist
            await CreatePineconeIndexAsync(pinecone, indexes, indexName);

            //get an index
            var index = await pinecone.GetIndex(indexName);

            return index;
        }

        private async Task<Vector[]> CreateVectorsAsync()
        {
            Task<List<VectorDataModel>> embeddings = CreateOpenAiEmbeddingsAsync();

            //create vectors to be upserted later into pinecone database
            List<Vector> vectorList = new();
            foreach (var embedding in await embeddings)
            {
                Vector vector = new Vector
                {
                    Id = embedding.FileName,
                    Metadata = new MetadataMap
                    {
                        ["name"] = embedding.FileName,
                        ["text"] = embedding.FileData,
                    },
                    Values = embedding.EmbeddingResult,

                };

                vectorList.Add(vector);
            }

            //amount of custom data will always change so Vector[] must be converted from list to array
            Vector[] vectors = vectorList.ToArray();

            return vectors;
        }

        private async Task<List<VectorDataModel>> CreateOpenAiEmbeddingsAsync()
        {
            OpenAIAPI openAi = _openAiService.GetOpenAI();

            //get custom data
            var customData = _customDataProvider.ReadCustomData();

            //create dictionary of embedding results by embedding custom data using openAi
            List<VectorDataModel> embeddingResults = new();
            foreach (var data in customData)
            {
                var result = await openAi.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(Model.AdaTextEmbedding, data.Value));

                VectorDataModel vectorData = new()
                {
                    FileName = data.Key,
                    FileData = data.Value,
                    EmbeddingResult = result,
                };
                embeddingResults.Add(vectorData);
            }

            return embeddingResults;
        }

        private async Task CreatePineconeIndexAsync(PineconeClient pinecone, IndexName[] indexes, string indexName)
        {
            //create new index if it doesn't exist
            if (!indexes.Contains(indexName))
            {
                await pinecone.CreateIndex(indexName, 1536, Metric.Cosine);
            }
        }

        private PineconeClient InitializePineCone()
        {
            //get pinecone API key
            var pineconeApiKey = _keyProvider.GetKey("pineconekey");

            var pineconeEnvironment = "asia-southeast1-gcp-free";
            var pinecone = new PineconeClient(pineconeApiKey, pineconeEnvironment);

            return pinecone;
        }
    }
}
