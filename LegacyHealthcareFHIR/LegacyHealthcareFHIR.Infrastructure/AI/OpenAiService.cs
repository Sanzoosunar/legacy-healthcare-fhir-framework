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
        var apiKey = configuration["OpenAI:ApiKey"]
           ?? throw new InvalidOperationException(
               "OpenAI API key is not configured.");

        _model = configuration["OpenAI:Model"]
            ?? throw new InvalidOperationException(
                "OpenAI model is not configured.");

        _client = new ResponsesClient(apiKey);
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        var response = await _client.CreateResponseAsync(_model, prompt);
        return response.Value.GetOutputText();
    }
}