using ChatBotCoach_logic.Clients.Contracts;

namespace ChatBotCoach_logic.Providers
{
    public class TextFileKeyProvider : IKeyProvider
    {
        //current keys: "openaikey", "pineconekey"
        private const string _apiKeysDir = @"C:\\Users\\Michael\\Desktop\\apikeys";

        public string GetKey(string keyName)
        {
            var filePath = Path.Combine(_apiKeysDir, $"{keyName}.txt");
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath).Trim();
            }

            throw new ArgumentException($"Key {keyName} not found.");
        }
    }
}
