namespace ChatBotCoachWebsite.Helpers.Services
{
    public interface IKeyProvider
    {
        string GetKey(string keyName);
    }

    public class TextFileKeyProvider : IKeyProvider
    {
        //current keys: "openaikey", "pineconekey"
        private readonly IConfiguration _config;
        public TextFileKeyProvider(IConfiguration config)
        {
            _config = config;
        }

        public string GetKey(string keyName)
        {
            var key = _config[$"Keys:{keyName}"];
            return key;
            throw new ArgumentException($"Key {keyName} not found.");
        }
    }
}
