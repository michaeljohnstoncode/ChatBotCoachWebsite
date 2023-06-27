namespace ChatBotCoachWebsite.Helpers.Services
{
    public interface ICustomDataProvider
    {
        Dictionary<string, string> ReadCustomData();
    }

    public class TextFileCustomDataProvider : ICustomDataProvider
    {
        private const string _customDataDir = @"C:\\Users\\Michael\\Desktop\\docs";

        public Dictionary<string, string> ReadCustomData()
        {
            //check if directory exists
            if (!Directory.Exists(_customDataDir))
            {
                Console.WriteLine($"Directory {_customDataDir} does not exist.");
                return null;
            }

            //get all .txt files in directory
            string[] files = Directory.GetFiles(_customDataDir, "*.txt");

            Dictionary<string, string> customData = new();
            //loop through and read all txt files
            foreach (string file in files)
            {
                string fileData = File.ReadAllText(file);
                string fileName = Path.GetFileName(file);
                customData.Add(fileName, fileData);
            }

            return customData;
        }
    }

}
