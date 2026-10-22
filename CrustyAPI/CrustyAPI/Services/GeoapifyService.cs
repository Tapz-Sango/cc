
using System.Text.Json;
using CrustyAPI.Models;

namespace CrustyAPI.Services
{
    public class GeoapifyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GeoapifyService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<AddressSuggestion>> SearchAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["Geoapify:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Geoapify API key is not configured.");
            }

            var parameters = new Dictionary<string, string>
            {
                ["text"] = query,
                ["filter"] = "countrycode:za",
                ["limit"] = "5",
                ["format"] = "json",
                ["apiKey"] = apiKey
            };

            var queryString = string.Join(
                "&",
                parameters.Select(p =>
                    $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

            var client = _httpClientFactory.CreateClient("Geoapify");

            using var response = await client.GetAsync(
                $"v1/geocode/autocomplete?{queryString}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                throw new HttpRequestException(
                    $"Geoapify returned {(int)response.StatusCode} " +
                    $"{response.StatusCode}. Details: {errorBody}");
            }

            await using var stream =
                await response.Content.ReadAsStreamAsync(cancellationToken);

            using var document = await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);

            var suggestions = new List<AddressSuggestion>();

            if (!document.RootElement.TryGetProperty(
                "results", out var results))
            {
                return suggestions;
            }

            foreach (var result in results.EnumerateArray())
            {
                suggestions.Add(new AddressSuggestion
                {
                    Formatted = GetString(result, "formatted"),
                    HouseNumber = GetString(result, "housenumber"),
                    Street = GetString(result, "street"),
                    Suburb = GetString(result, "suburb"),
                    District = GetString(result, "district"),
                    City = GetString(result, "city")
                });
            }

            return suggestions;
        }

        private static string GetString(
            JsonElement element,
            string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var value) &&
                value.ValueKind == JsonValueKind.String)
            {
                return value.GetString() ?? "";
            }

            return "";
        }
    }
}
