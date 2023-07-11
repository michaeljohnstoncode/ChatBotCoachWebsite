using ChatBotCoach_data.models;
using Microsoft.Extensions.Logging;

namespace ChatBotCoach_logic.Clients.Contracts
{
    public abstract class BaseHttpClient<T>
    {
        protected const string AuthType = "Bearer";

        protected ILogger Loggger;

        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenClaimService _tokenClaimService;
        private readonly RequestSettingsModel _requestSettings;

        protected BaseHttpClient(ILogger<T> logger,
            IHttpClientFactory clientFactory,
            ITokenClaimService tokenClaimService,
            RequestSettingsModel requestSettings)
        {
            Loggger = logger;
            _clientFactory = clientFactory;
            _tokenClaimService = tokenClaimService;
            _requestSettings = requestSettings;
        }

        /// <summary>
        /// This method is testable way to obtain the security token.
        /// </summary>
        /// <exception cref="SecurityException">Can throw if the token is unable to be retrieved or is empty/null.</exception>
        protected async Task<TokenModel> GetSecurityToken(CancellationToken cancellationToken)
        {
            var securityToken = await _tokenClaimService.GetTokenAsync(_requestSettings, cancellationToken);

            if (securityToken == null)
                throw new SecurityException("Unable to retrieve security token.");

            if (string.IsNullOrEmpty(securityToken.AccessToken))
                throw new SecurityException("Access token is empty.");

            return securityToken;
        }
        /// <summary>
        /// Ensures the client is created and associated with the correct name.
        /// </summary>
        protected HttpClient CreateClient(string name)
        {
            if (_clientFactory == null)
                return new HttpClient();

            if (string.IsNullOrEmpty(name))
                return _clientFactory.CreateClient();

            return _clientFactory.CreateClient(name);
        }
    }
}
