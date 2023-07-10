using ChatBotCoachWebsite.Helpers.Services;
using ChatBotCoachWebsite.Models;
using OpenAI_API;
using OpenAI_API.Embedding;
using OpenAI_API.Models;
using Pinecone;
using Pinecone.Grpc;


namespace ChatBotCoachWebsite.Helpers
{
    public class GetPineconeIndex
    {
        private IKeyProvider _keyProvider;

        public GetPineconeIndex(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        public async Task<Index<GrpcTransport>> GetPineconeIndexAsync()
        {
            //get instance of pinecone client
            var pinecone = InitializePineCone();

            //index name (only have 1 index for now)
            var indexName = "compcoachindex";

            //get an index
            var index = await pinecone.GetIndex(indexName);

            return index;
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
