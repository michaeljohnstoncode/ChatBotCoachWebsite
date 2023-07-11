using ChatBotCoach_data.models;
using ChatBotCoach_logic.Clients.Contracts;

namespace ChatBotCoach_logic
{
    public class TextTokenClaimService : ITokenClaimService
    {
        IKeyProvider _keyProvider;

        public TextTokenClaimService(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }
        public Task<TokenModel> GetTokenAsync(RequestSettingsModel request, CancellationToken cancellationToken)
        {
            // pretends to be async, same result as just calling it directly but returns as a task.
            return Task.Run(() =>
                {
                    return TokenModel.FromText(GetKey(request.Name));
                }, cancellationToken);
        }

        public string GetKey(string keyName)
        {
            return _keyProvider.GetKey(keyName);
        }
    }
}
