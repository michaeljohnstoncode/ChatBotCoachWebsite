using OpenAI_API;
using OpenAI_API.Embedding;
using OpenAI_API.Images;
using OpenAI_API.Models;
using Pinecone;
using Pinecone.Grpc;
using static ChatBotCoachWebsite.Helpers.IKeyProvider;

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
        private IKeyProvider _keyService;

        public BuildPineconeIndex(ICustomDataProvider customDataProvider, IKeyProvider keyService)
        {
            _customDataProvider = customDataProvider;
            _keyService = keyService;
        }

        //TODO: after building the pinecone index, next it needs to be queried for relevant context. relevant context may come from the user's question text. 
        //      once relevant context is retrieved from index, then openai prompt may be created.
        //      prompt("something something coach") + indexContext + user's question ---> insert to openai completion --->  returndeliver ai response

        public async Task UpsertPineconeIndexAsync()
        {
            //get vectors for index
            Vector[] vectors = await CreateVectorsAsync();

            //create or get pinecone index
            Index<GrpcTransport> index = await CreatePineconeIndexAsync();

            //configure index
            //Configuring indexes is NOT supported in free tier.
            //calling ConfigureIndex is not necessary unless explicitly required. creation of index comes with default configuration
            //await pinecone.ConfigureIndex(indexName, replicas: 1, podType: "p1");

            //upsert vectors into index
            //await index.Upsert(vectors);

            var filter = new MetadataMap
            {
                ["text"] = new MetadataMap
                {
                    ["$in"] = new MetadataValue[] { "farah" }
                }
            };

            //get openAI api key
            var openAiApiKey = _keyService.GetKey("openaikey");
            //initialize openAi
            OpenAIAPI openAi = new OpenAIAPI(openAiApiKey);
            var embeddingQuery = await openAi.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(Model.AdaTextEmbedding, "farah"));
            var qery = await index.Query(embeddingQuery, topK: 5, includeMetadata: true);
            foreach(var vector in qery)
            {
                var test = vector.Metadata;
            }
            //var fetch = await index.Fetch(new[] { "mercy.txt" });
           // var query = await index.Query("mercy.txt", topK: 5, filter);
            
           // var qtest = query.GetValue(0);
        }

        private PineconeClient InitializePineCone()
        {
            //get pinecone API key
            var pineconeApiKey = _keyService.GetKey("pineconekey");

            var pineconeEnvironment = "asia-southeast1-gcp-free";
            var pinecone = new PineconeClient(pineconeApiKey, pineconeEnvironment);

            return pinecone;
        }

		private async Task<List<VectorDataModel>> CreateOpenAiEmbeddingsAsync()
		{
            //get openAI api key
            var openAiApiKey = _keyService.GetKey("openaikey");

            //get custom data
            var customData = _customDataProvider.ReadCustomData();

            //initialize openAi
            OpenAIAPI openAi = new OpenAIAPI(openAiApiKey);

            //create dictionary of embedding results by embedding custom data using openAi
            List<VectorDataModel> embeddingResults = new();
			foreach(var data in customData)
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

        private async Task<Index<GrpcTransport>> CreatePineconeIndexAsync()
        {
            //get instance of pinecone client
            var pinecone = InitializePineCone();

            //get list of pinecone indexes
            var indexes = await pinecone.ListIndexes();

            //create new index if it doesn't exist
            var indexName = "compcoachindex";
            if (!indexes.Contains(indexName))
            {
                await pinecone.CreateIndex(indexName, 1536, Metric.Cosine);
            }

            //get an index
            var index = await pinecone.GetIndex(indexName);

            return index;
        }
    }
}

/*
  1. Build a Knowledge Base:
     -initialize Pinecone connection
     -create a new index
     -upsert data to index

 2. Retrieval
     -initialize OpenAI
     -to search thru data, first create query vector xq
     -initialize OpenAI embedding model (query, engineModel)"text-embedding-ada-002"
     -retrieve data from pinecone (index?) using xq
     -query index using relevant context
     -build OpenAI prompt -- prompt_start = prompt + context
                             prompt_end = user question

     -create OpenAI completion. function CompletePrompt(openAiPrompt).
                                completion model="text-davinci-003"	 
                                return answer

     -once were done with index delete it to save resources
     (Why delete it? Will my data still be saved for later use?)

  */
