using Pinecone;

namespace ChatBotCoachWebsite.Helpers
{
    public class BuildKnowledgeBase
    {
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
