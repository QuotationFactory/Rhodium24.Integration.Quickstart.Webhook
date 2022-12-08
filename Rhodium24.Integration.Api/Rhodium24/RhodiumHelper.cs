using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Versioned.ExternalDataContracts;
using Versioned.ExternalDataContracts.Contracts.Project;

namespace Rhodium24.Integration.Api.Rhodium24
{
    public class RhodiumHelper
    {
        private readonly RhodiumSettings _settings;
        private readonly RestClient _restClient;
        private readonly JsonSerializerSettings _projectJsonSerializerSettings;

        public RhodiumHelper(IOptions<RhodiumSettings> settings)
        {
            _settings = settings.Value;
            _restClient = new RestClient(settings.Value.ApiUrl);
            _restClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", settings.Value.SubscriptionKey);

            _projectJsonSerializerSettings = new JsonSerializerSettings();
            _projectJsonSerializerSettings.SetJsonSettings();
            _projectJsonSerializerSettings.AddJsonConverters();
            _projectJsonSerializerSettings.SerializationBinder = new CrossPlatformTypeBinder();
        }

        private async Task<string> RetrieveAccessToken()
        {
            if (_settings == null) throw new ArgumentNullException(nameof(_settings), "Rhodium settings object is null");
            if (string.IsNullOrEmpty(_settings.TokenUrl)) throw new ArgumentNullException(nameof(_settings.TokenUrl), "Rhodium settings token URL is not set");
            if (string.IsNullOrEmpty(_settings.ClientId)) throw new ArgumentNullException(nameof(_settings.ClientId), "Rhodium settings client id is not set");
            if (string.IsNullOrEmpty(_settings.ClientSecret)) throw new ArgumentNullException(nameof(_settings.ClientSecret), "Rhodium settings client secret is not set");
            if (string.IsNullOrEmpty(_settings.Audience)) throw new ArgumentNullException(nameof(_settings.Audience), "Rhodium settings audience is not set");

            var client = new RestClient(_settings.TokenUrl);
            var request = new RestRequest();
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={_settings.ClientId}&client_secret={_settings.ClientSecret}&audience={_settings.Audience}", ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request, Method.Post);

            if (!response.IsSuccessful)
                throw new Exception("Error retrieving new access token", response.ErrorException);

            var responseDefinition = new
            {
                access_token = string.Empty,
                expires_in = 0,
                scope = string.Empty,
                token_type = string.Empty
            };

            var responseObject = JsonConvert.DeserializeAnonymousType(response.Content, responseDefinition);
            return responseObject.access_token;
        }

        public async Task<byte[]> GetDocument(Guid partyId, Guid projectId, string fileName)
        {
            var token = await RetrieveAccessToken();

            var request = new RestRequest($"parties/{partyId}/projects/{projectId}/documents/{fileName}");
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = await _restClient.ExecuteAsync(request, Method.Get);

            if (!response.IsSuccessful)
                throw new Exception($"Error while retrieving document partyId: {partyId}, projectId:{projectId}, fileName:{fileName}");

            return Convert.FromBase64String(response.Content);
        }

        public async Task<ProjectV1> GetProject(Guid partyId, Guid projectId)
        {
            var token = await RetrieveAccessToken();

            var request = new RestRequest($"parties/{partyId}/projects/{projectId}");
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = await _restClient.ExecuteAsync(request, Method.Get);

            if (!response.IsSuccessful)
                throw new Exception($"Error while retrieving project partyId: {partyId}, projectId:{projectId}");

            var project = JsonConvert.DeserializeObject<ProjectV1>(response.Content, _projectJsonSerializerSettings);
            return project;
        }
    }
}
