// Copyright (c) Microsoft. All rights reserved.

// This sample demonstrates how to create a minimal API that uses AgentCatalog to retrieve all registered agents
// and return their metadata including custom additional properties.

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add a chat client to the service collection
builder.Services.AddSingleton(sp => new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
        .GetChatClient(deploymentName)
        .AsIChatClient());

// Configure multiple agents with additional properties
builder.AddAIAgent("weather-agent", (sp, key) =>
{
    IChatClient chatClient = sp.GetRequiredService<IChatClient>();
    ChatClientAgentOptions options = new ChatClientAgentOptions
    {
        Name = key,
        Instructions = "You are a helpful weather assistant that provides weather information.",
        Description = "An agent that helps users with weather-related queries.",
        AdditionalProperties = new AdditionalPropertiesDictionary
        {
            ["icon"] = "https://example.com/icons/weather.png",
            ["beta"] = false,
            ["visibility"] = "Visible"
        }
    };
    return new ChatClientAgent(chatClient, options);
});

builder.AddAIAgent("travel-agent", (sp, key) =>
{
    IChatClient chatClient = sp.GetRequiredService<IChatClient>();
    ChatClientAgentOptions options = new ChatClientAgentOptions
    {
        Name = key,
        Instructions = "You are a helpful travel assistant that helps plan trips.",
        Description = "An agent that helps users plan their travel and vacations.",
        AdditionalProperties = new AdditionalPropertiesDictionary
        {
            ["icon"] = "https://example.com/icons/travel.png",
            ["beta"] = true,
            ["visibility"] = "Visible"
        }
    };
    return new ChatClientAgent(chatClient, options);
});

builder.AddAIAgent("experimental-agent", (sp, key) =>
{
    IChatClient chatClient = sp.GetRequiredService<IChatClient>();
    ChatClientAgentOptions options = new ChatClientAgentOptions
    {
        Name = key,
        Instructions = "You are an experimental assistant for testing new features.",
        Description = "An experimental agent for internal testing only.",
        AdditionalProperties = new AdditionalPropertiesDictionary
        {
            ["icon"] = "https://example.com/icons/experimental.png",
            ["beta"] = true,
            ["visibility"] = "Unlisted"
        }
    };
    return new ChatClientAgent(chatClient, options);
});

WebApplication app = builder.Build();

app.MapGet("/agents", async (AgentCatalog agentCatalog, CancellationToken cancellationToken) =>
{
    List<object> agents = new List<object>();

    await foreach (AIAgent agent in agentCatalog.GetAgentsAsync(cancellationToken))
    {
        agents.Add(new { name = agent.Name, properties = agent.AdditionalProperties });
    }

    return agents;
});

app.Run();
