namespace ChatBotCoach_logic.Clients.Contracts
{
    public interface IKeyProvider
    {
        string GetKey(string keyName);
    }
}
