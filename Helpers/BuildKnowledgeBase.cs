using OpenAI_API;
using OpenAI_API.Embedding;
using OpenAI_API.Models;
using Pinecone;
using Pinecone.Grpc;

namespace ChatBotCoachWebsite.Helpers
{
    public class BuildKnowledgeBase
    {
		
		private PineconeClient InitializePineCone()
		{
            var pineconeApiKey = "";
			var pineconeEnvironment = "";
			using var pinecone = new PineconeClient(pineconeApiKey, pineconeEnvironment);
			return pinecone;
        }
		
		private OpenAIAPI InitializeOpenAi()
		{
			OpenAIAPI openAi = new OpenAIAPI("");
			return openAi;
		}

		private async Task<Index<GrpcTransport>> CreatePineConeIndexAsync()
		{
			PineconeClient pinecone = InitializePineCone();

			var indexes = await pinecone.ListIndexes();

			//create new index if it doesn't exist
			var indexName = "coachKnowledge";
			if (!indexes.Contains(indexName))
			{
				await pinecone.CreateIndex(indexName, 1536, Metric.Cosine);
			}

			//get an index
			using var index = await pinecone.GetIndex(indexName);

			//configure index
			await pinecone.ConfigureIndex(indexName, replicas: 1, podType: "p1");

			return index;
		}

		private Dictionary<string, string> ReadCustomData()
		{
			var dir = "C:\\Users\\Michael\\Desktop\\docs";

			//check if directory exists
			if (!Directory.Exists(dir))
			{
				Console.WriteLine($"Directory {dir} does not exist.");
				return null;
			}

			//get all .txt files in directory
			string[] files = Directory.GetFiles(dir, "*.txt");

			Dictionary<string, string> customData = new();
			//loop through and read all txt files
			foreach(string file in files)
			{
				string fileData = File.ReadAllText(file);
				string fileName = Path.GetFileName(file);
				customData.Add(fileName, fileData);
			}

			return customData;
        }

		private async Task<Dictionary<string, EmbeddingResult>> CreateOpenAiEmbeddingsAsync()
		{
			//initialize openAi
			OpenAIAPI openAi = InitializeOpenAi();
			//get custom data
            Dictionary<string, string> customData = ReadCustomData();

			//create dictionary of embedding results by embedding custom data using openAi
			Dictionary<string, EmbeddingResult> embeddingResults = new();
			foreach(var data in customData)
			{
				var result = await openAi.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest(data.Value, Model.AdaTextEmbedding));
				embeddingResults.Add(data.Key, result);
			}

			return embeddingResults;
		}

		private async Task CreateUpsertVectorsAsync()
		{
			Task<Index<GrpcTransport>> index = CreatePineConeIndexAsync();
            Task<Dictionary<string, EmbeddingResult>> embeddings = CreateOpenAiEmbeddingsAsync();

			//create vectors to upsert into pinecone database
			Vector[] vectors = new { };
			foreach(var embedding in await embeddings)
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

    }
}
