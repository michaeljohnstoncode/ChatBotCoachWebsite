using ChatBotCoach_data.models;
namespace ChatBotCoach_logic.Clients.Contracts
{
    public interface ITokenClaimService
    {
        public Task<TokenModel> GetTokenAsync(RequestSettingsModel request, CancellationToken cancellationToken);
    }
}
