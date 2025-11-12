// Copyright (c) Microsoft. All rights reserved.

// This sample demonstrates how to create a minimal API that uses AgentCatalog to retrieve all registered agents
// and return their metadata including custom additional properties.

using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using System.Text.Json.Serialization;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o-mini";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure OpenAPI/Swagger
builder.Services.AddOpenApi();

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

// Configure the HTTP request pipeline
app.MapOpenApi();

// Create the /agents endpoint
app.MapGet("/agents", async (AgentCatalog agentCatalog, CancellationToken cancellationToken) =>
{
    List<AgentInfo> agents = new List<AgentInfo>();

    await foreach (AIAgent agent in agentCatalog.GetAgentsAsync(cancellationToken))
    {
        AgentInfo agentInfo = new AgentInfo
        {
            Name = agent.Name ?? "Unknown"
        };

        // Extract additional properties if they exist
        if (agent.AdditionalProperties is not null)
        {
            if (agent.AdditionalProperties.TryGetValue("icon", out object? iconValue) && iconValue is string icon)
            {
                agentInfo.Icon = icon;
            }

            if (agent.AdditionalProperties.TryGetValue("beta", out object? betaValue) && betaValue is bool beta)
            {
                agentInfo.Beta = beta;
            }

            if (agent.AdditionalProperties.TryGetValue("visibility", out object? visibilityValue) && visibilityValue is string visibilityString &&
                Enum.TryParse<AgentVisibility>(visibilityString, out AgentVisibility visibility))
            {
                agentInfo.Visibility = visibility;
            }
        }

        agents.Add(agentInfo);
    }

    return Results.Ok(agents);
})
.WithName("GetAgents")
.WithOpenApi();

app.Run();

/// <summary>
/// Represents information about an agent including its metadata and additional properties.
/// </summary>
internal sealed class AgentInfo
{
    /// <summary>
    /// Gets or sets the name of the agent.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the URL to the agent's icon.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets whether the agent is in beta.
    /// </summary>
    public bool Beta { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the agent.
    /// </summary>
    public AgentVisibility Visibility { get; set; }
}

/// <summary>
/// Defines the visibility options for an agent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum AgentVisibility
{
    /// <summary>
    /// The agent is visible to all users.
    /// </summary>
    Visible,

    /// <summary>
    /// The agent is unlisted and only accessible via direct reference.
    /// </summary>
    Unlisted
}
