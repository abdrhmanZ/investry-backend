using Investry.Application.Contracts.Infrastructure;
using Investry.Application.DTOs;
using Investry.Application.Models.Kyc;
using Investry.Infrastructure.Kyc.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Investry.Infrastructure.Kyc.Providers
{
    public class DiditKycProvider : IKycProvider
    {
        private readonly HttpClient _httpClient;
        private readonly DiditSettings _settings;
        public DiditKycProvider(HttpClient httpClient, IOptions<DiditSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
        }
        public async Task<StartKycSessionResponse> CreateSessionAsync(string vendorData)
        {
            var requestBody = new DiditCreateSessionRequest
            {
                workflow_id = _settings.WorkflowId,
                vendor_data = vendorData,
                callback = _settings.CallbackUrl
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_settings.BaseUrl}/v3/session", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var diditResponse = JsonSerializer.Deserialize<DiditCreateSessionResponse>(json);

            return new StartKycSessionResponse
            {
                SessionId = diditResponse.session_id,
                VerificationUrl = diditResponse.url
            };
        }

        public async Task<StartKycSessionResponse> RetrieveSessionAsync(string sessionId)
        {
            var response = await _httpClient.GetAsync($"{_settings.BaseUrl}/v3/session/{sessionId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var diditResponse = JsonSerializer.Deserialize<JsonElement>(json);

            return new StartKycSessionResponse
            {
                SessionId = diditResponse.GetProperty("session_id").GetString(),
                VerificationUrl = diditResponse.GetProperty("url").GetString()
            };
        }
    }
}
