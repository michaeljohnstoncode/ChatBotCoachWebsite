namespace ChatBotCoach_config.Contracts
{
    // Allows client configuration to wrap another class to specify who it belongs to.
    public interface IClientConfiguration<T> where T : class
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
    }
}
