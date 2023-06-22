using OpenAI_API;
using OpenAI_API.Embedding;
using OpenAI_API.Models;
using Pinecone;
using Pinecone.Grpc;
using static ChatBotCoachWebsite.Helpers.IKeyProvider;

namespace ChatBotCoachWebsite.Helpers
{
    public class BuildPineconeIndex
    {
        private ICustomDataProvider _customDataProvider;
        private KeyService _keyService;

        public BuildPineconeIndex(ICustomDataProvider customDataProvider, KeyService keyService)
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
            await index.Upsert(vectors);
        }

        private PineconeClient InitializePineCone()
        {
            //get pinecone API key
            var pineconeApiKey = _keyService.GetApiKey("pineconekey");

            var pineconeEnvironment = "asia-southeast1-gcp-free";
            var pinecone = new PineconeClient(pineconeApiKey, pineconeEnvironment);

            return pinecone;
        }

		private async Task<Dictionary<string, EmbeddingResult>> CreateOpenAiEmbeddingsAsync()
		{
            //get openAI api key
            var openAiApiKey = _keyService.GetApiKey("openaikey");

            //get custom data
            var customData = _customDataProvider.ReadCustomData();

            //initialize openAi
            OpenAIAPI openAi = new OpenAIAPI(openAiApiKey);

            //create dictionary of embedding results by embedding custom data using openAi
            Dictionary<string, EmbeddingResult> embeddingResults = new();
			foreach(var data in customData)
			{
				var result = await openAi.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(Model.AdaTextEmbedding, data.Value));
				embeddingResults.Add(data.Key, result);
			}

			return embeddingResults;
		}

        private async Task<Vector[]> CreateVectorsAsync()
        {
            Task<Dictionary<string, EmbeddingResult>> embeddings = CreateOpenAiEmbeddingsAsync();

            //create vectors to be upserted later into pinecone database
            List<Vector> vectorList = new();
            foreach (var embedding in await embeddings)
            {
                Vector vector = new Vector
                {
                    Id = embedding.Key,
                    Metadata = new MetadataMap
                    {
                        ["name"] = embedding.Key
                    },
                    Values = embedding.Value,
					
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
