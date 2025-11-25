using System.Net.Http.Json;
using System.Text;
using LogAnalyzerData.Models;

namespace LogAnalyzerBusiness.Services.Gpt;

public class GptService(HttpClient httpClient) : IGptService
{
    public async Task<string> AnalyzeLogs(ParsedLogs logs)
    {
        var prompt = BuildPrompt(logs);

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "user", content = prompt },
            },
            max_tokens = 1000
        };
        var response = await httpClient.PostAsJsonAsync("/v1/chat/completions",requestBody);
        
        var result = await response.Content.ReadFromJsonAsync<GptResponse>();
        
        return result?.Choices[0].Message.Content ?? "No summary";
    }

    private string BuildPrompt( ParsedLogs logs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Analyze these logs and provide a concise summary:");
        sb.AppendLine($"Total entries: {logs.Entries.Count}");
        
        foreach (var kvp in logs.CountByLevel)
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
    
        sb.AppendLine("\nProvide: 1) Main issues 2) Severity 3) Recommendations");
    
        return sb.ToString();
    }
}