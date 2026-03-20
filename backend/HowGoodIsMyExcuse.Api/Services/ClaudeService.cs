using System.Text;
using System.Text.Json;
using HowGoodIsMyExcuse.Api.DTOs;

namespace HowGoodIsMyExcuse.Api.Services;

public class ClaudeService : IClaudeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;

    public ClaudeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["Anthropic:ApiKey"]
            ?? throw new InvalidOperationException("Anthropic:ApiKey is not configured.");
    }

    public async Task<JudgeResult> EvaluateExcuse(string excuseText, string judgePersonality)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

            var prompt = $$"""
            You are a judge with the following personality: {{judgePersonality}}.
            A user has submitted this excuse: "{{excuseText}}"
            Evaluate this excuse and respond ONLY with a valid JSON object in exactly this format, no extra text, no markdown:
            {"score": <integer 0-100>, "verdict": "<one sentence verdict>", "roast": "<two to three sentence roast>"}
            Score 0-20 means the excuse is terrible, 21-50 means weak, 51-80 means acceptable, 81-100 means brilliant.
            """;

            var requestBody = new
            {
                model = "claude-sonnet-4-20250514",
                max_tokens = 300,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.anthropic.com/v1/messages", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var text = doc.RootElement
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString()
                ?? throw new ClaudeServiceException("Empty response from Claude.");

            var result = JsonSerializer.Deserialize<JudgeResult>(text, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new ClaudeServiceException("Failed to deserialize Claude response.");

            return result;
        }
        catch (ClaudeServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ClaudeServiceException($"Claude API call failed: {ex.Message}");
        }
    }
}