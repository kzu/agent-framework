# Agent Catalog Minimal API Sample

This sample demonstrates how to create a minimal API application that uses the `AgentCatalog` to retrieve all registered AI agents and expose their metadata through a REST endpoint.

## Overview

The sample creates an absolutely minimal ASP.NET Core web API with:
- Multiple AI agents registered with custom metadata
- An `/agents` endpoint that returns a JSON array of all agents with their names and additional properties

## Features

- **AgentCatalog Integration**: Uses the `AgentCatalog` to enumerate all registered agents
- **Custom Metadata**: Each agent includes additional properties stored in `AdditionalProperties` dictionary
- **Simple API**: Returns anonymous objects with agent name and properties - clients can handle the data as needed

## Prerequisites

- .NET 9.0 or later
- Azure OpenAI resource with a deployed model
- Azure CLI for authentication

## Configuration

Set the following environment variables:

```bash
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_DEPLOYMENT_NAME="gpt-4o-mini"
```

## Running the Sample

1. Navigate to the sample directory:
   ```bash
   cd dotnet/samples/HostedAgents/AgentCatalogMinimalAPI
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Test the `/agents` endpoint:
   ```bash
   curl http://localhost:5000/agents
   ```

## Example Response

```json
[
  {
    "name": "weather-agent",
    "properties": {
      "icon": "https://example.com/icons/weather.png",
      "beta": false,
      "visibility": "Visible"
    }
  },
  {
    "name": "travel-agent",
    "properties": {
      "icon": "https://example.com/icons/travel.png",
      "beta": true,
      "visibility": "Visible"
    }
  },
  {
    "name": "experimental-agent",
    "properties": {
      "icon": "https://example.com/icons/experimental.png",
      "beta": true,
      "visibility": "Unlisted"
    }
  }
]
```

## Code Structure

The sample demonstrates:

1. **Agent Registration**: Multiple agents are registered using `builder.AddAIAgent()` with custom additional properties
2. **AgentCatalog Usage**: The `AgentCatalog` service is injected into the endpoint handler
3. **Simple Response**: Returns anonymous objects with `name` and `properties` - clients handle parsing as needed

## Key Concepts

- **AgentCatalog**: Provides enumeration of all registered agents in the application
- **AdditionalProperties**: A dictionary that can store custom metadata on agents
- **Minimal APIs**: Uses ASP.NET Core minimal API syntax for clean, concise endpoint definitions
- **Dependency Injection**: Agents are registered in DI and resolved through the `AgentCatalog`
