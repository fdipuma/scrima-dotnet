using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Scrima.Integration.Tests.Models;

namespace Scrima.Integration.Tests.Utility;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<QueryResultDto<T>> GetQueryAsync<T>(
        this HttpClient client,
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync(requestUri, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var jsonText = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Request failed with status code {response.StatusCode} and response {jsonText}");
        }

        try
        {
            await using var json = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<QueryResultDto<T>>(json, SerializerOptions, cancellationToken: cancellationToken);
        }
        catch (JsonException e)
        {
            var jsonText = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to deserialize JSON: '{jsonText}', {e}");
        }
    }
}
