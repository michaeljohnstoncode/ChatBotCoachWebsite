using OpenAI_API.Embedding;

namespace ChatBotCoachWebsite.Models
{
    public class VectorData
    {
        public string FileName { get; set; }
        public string FileData { get; set; }
        public EmbeddingResult EmbeddingResult { get; set; }
    }

}
