using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace prototipo2.Services
{
    public class PayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpires;

        public PayPalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && _tokenExpires > DateTime.UtcNow.AddMinutes(1))
                return _accessToken;

            var clientId = _configuration["PayPal:ClientId"];
            var secret = _configuration["PayPal:ClientSecret"];
            var env = _configuration["PayPal:Environment"] ?? "sandbox";

            var baseUrl = env == "live" ? "https://api-m.paypal.com" : "https://api-m.sandbox.paypal.com";

            var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            request.Content = new StringContent("grant_type=client_credentials", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(responseStream);
            _accessToken = json.RootElement.GetProperty("access_token").GetString();
            int expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();
            _tokenExpires = DateTime.UtcNow.AddSeconds(expiresIn);

            return _accessToken;
        }

        public async Task<bool> ValidarOrdenAsync(string orderId)
        {
            var env = _configuration["PayPal:Environment"] ?? "sandbox";
            var baseUrl = env == "live" ? "https://api-m.paypal.com" : "https://api-m.sandbox.paypal.com";

            var accessToken = await GetAccessTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/v2/checkout/orders/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return false;

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(responseStream);

            var status = json.RootElement.GetProperty("status").GetString();

            return status == "COMPLETED";
        }
    }
}
