using System.Text;
using System.Text.Json;
using HowGoodIsMyExcuse.Api.DTOs;

namespace HowGoodIsMyExcuse.Api.Services;

public class GroqService : IGroqService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;

    public GroqService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["Groq:ApiKey"]
            ?? throw new InvalidOperationException("Groq:ApiKey is not configured.");
    }

    public async Task<JudgeResult> EvaluateExcuse(string excuseText, string judgePersonality)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var prompt = $$"""
                You are a judge with the following personality: {{judgePersonality}}.
                A user has submitted this excuse: "{{excuseText}}"
                Evaluate this excuse and respond ONLY with a valid JSON object in exactly this format, no extra text, no markdown:
                {"score": <integer 0-100>, "verdict": "<one sentence verdict>", "roast": "<two to three sentence roast>"}
                Score 0-20 means the excuse is terrible, 21-50 means weak, 51-80 means acceptable, 81-100 means brilliant.
                """;

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                max_tokens = 300,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"=== GROQ RAW RESPONSE ===");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Body: {responseBody}");
            Console.WriteLine($"=========================");

            if (!response.IsSuccessStatusCode)
                throw new GroqServiceException($"Groq API returned {response.StatusCode}: {responseBody}");

            using var doc = JsonDocument.Parse(responseBody);
            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? throw new GroqServiceException("Empty response from Groq.");

            var result = JsonSerializer.Deserialize<JudgeResult>(text, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new GroqServiceException("Failed to deserialize Groq response.");

            return result;
        }
        catch (GroqServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new GroqServiceException($"Groq API call failed: {ex.Message}");
        }
    }
}