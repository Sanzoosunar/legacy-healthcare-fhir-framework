#pragma warning disable OPENAI001

using LegacyHealthcareFHIR.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenAI.Responses;

namespace LegacyHealthcareFHIR.Infrastructure.AI;

public class OpenAiService : IAiService
{
    private readonly ResponsesClient _client;
    private readonly string _model;

    public OpenAiService(IConfiguration configuration)
    {
        var apiKeyEnvironmentName = configuration["OpenAI:ApiKey"];
        var apiKey = Environment.GetEnvironmentVariable(apiKeyEnvironmentName!);

        _model = configuration["OpenAI:Model"]!;
      
        _client = new ResponsesClient(apiKey);
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        var response = await _client.CreateResponseAsync(_model, prompt);
        return response.Value.GetOutputText();
    }
}