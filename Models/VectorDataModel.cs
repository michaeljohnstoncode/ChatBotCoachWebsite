using OpenAI_API.Embedding;

namespace ChatBotCoachWebsite.Models
{
    public class VectorDataModel
    {
        public string FileName { get; set; }
        public string FileData { get; set; }
        public EmbeddingResult EmbeddingResult { get; set; }
    }

}
