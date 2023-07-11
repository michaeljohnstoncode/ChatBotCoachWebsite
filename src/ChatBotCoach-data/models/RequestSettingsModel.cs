namespace ChatBotCoach_data.models
{
    public class RequestSettingsModel
    {
        public string Name { get; set; }
        public RequestSettingsModel(string name)
        {
            Name = name;
        }
    }
}
