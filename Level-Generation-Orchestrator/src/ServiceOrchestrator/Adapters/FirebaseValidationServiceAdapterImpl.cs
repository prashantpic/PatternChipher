using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using PatternCipher.Services.Configuration; // For OrchestratorSettings
using PatternCipher.Services.Exceptions; // For ServerValidationFailedException
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System; // For ArgumentNullException
using UnityEngine; // For Debug.Log

namespace PatternCipher.Services.Adapters
{
    public class FirebaseValidationServiceAdapterImpl : IFirebaseValidationServiceAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly OrchestratorSettings _settings;

        public FirebaseValidationServiceAdapterImpl(HttpClient httpClient, OrchestratorSettings settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<LevelValidationResult> ValidateLevelOnServerAsync(LevelValidationRequest validationRequest)
        {
            if (validationRequest == null) throw new ArgumentNullException(nameof(validationRequest));
            if (string.IsNullOrEmpty(_settings.FirebaseValidationFunctionUrl))
            {
                Debug.LogError("[FirebaseValidationServiceAdapterImpl] FirebaseValidationFunctionUrl is not configured in OrchestratorSettings.");
                throw new InvalidOperationException("FirebaseValidationFunctionUrl is not configured.");
            }

            Debug.Log($"[FirebaseValidationServiceAdapterImpl] Sending level validation request to: {_settings.FirebaseValidationFunctionUrl}");

            try
            {
                string requestBody = JsonConvert.SerializeObject(validationRequest);
                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_settings.FirebaseValidationFunctionUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.LogError($"[FirebaseValidationServiceAdapterImpl] Server validation HTTP error: {response.StatusCode}. Response: {errorContent}");
                    throw new ServerValidationFailedException($"Server validation HTTP request failed with status {response.StatusCode}. Details: {errorContent}");
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                LevelValidationResult result = JsonConvert.DeserializeObject<LevelValidationResult>(responseBody);
                
                Debug.Log($"[FirebaseValidationServiceAdapterImpl] Server validation response received. IsValid: {result?.IsValid}");
                return result ?? new LevelValidationResult { IsValid = false, Reason = "Null response from server."};
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"[FirebaseValidationServiceAdapterImpl] HTTP request exception during server validation: {ex.Message}");
                throw new ServerValidationFailedException("Network error during server validation.", ex);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[FirebaseValidationServiceAdapterImpl] JSON exception during server validation request/response: {ex.Message}");
                throw new ServerValidationFailedException("JSON processing error during server validation.", ex);
            }
            catch (Exception ex)
            {
                 Debug.LogError($"[FirebaseValidationServiceAdapterImpl] Unexpected error during server validation: {ex.Message}");
                throw new ServerValidationFailedException("Unexpected error during server validation.", ex);
            }
        }
    }
}