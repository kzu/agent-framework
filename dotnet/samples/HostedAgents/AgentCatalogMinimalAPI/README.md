# Agent Catalog Minimal API Sample

This sample demonstrates how to create a minimal API application that uses the `AgentCatalog` to retrieve all registered AI agents and expose their metadata through a REST endpoint.

## Overview

The sample creates a minimal ASP.NET Core web API with:
- Multiple AI agents registered with custom metadata
- An `/agents` endpoint that returns a JSON list of all agents
- Custom additional properties for each agent (icon URL, beta status, visibility)

## Features

- **AgentCatalog Integration**: Uses the `AgentCatalog` to enumerate all registered agents
- **Custom Metadata**: Each agent includes additional properties:
  - `icon`: A URL pointing to the agent's icon
  - `beta`: A boolean indicating if the agent is in beta
  - `visibility`: An enum (`Visible` or `Unlisted`) indicating the agent's visibility
- **RESTful API**: Exposes a GET `/agents` endpoint that returns agent information as JSON
- **OpenAPI/Swagger**: Includes OpenAPI documentation for easy API exploration

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

3. Open a browser and navigate to the OpenAPI documentation:
   ```
   http://localhost:5000/openapi/v1.json
   ```

4. Test the `/agents` endpoint:
   ```bash
   curl http://localhost:5000/agents
   ```

## Example Response

```json
[
  {
    "name": "weather-agent",
    "icon": "https://example.com/icons/weather.png",
    "beta": false,
    "visibility": "Visible"
  },
  {
    "name": "travel-agent",
    "icon": "https://example.com/icons/travel.png",
    "beta": true,
    "visibility": "Visible"
  },
  {
    "name": "experimental-agent",
    "icon": "https://example.com/icons/experimental.png",
    "beta": true,
    "visibility": "Unlisted"
  }
]
```

## Code Structure

The sample demonstrates:

1. **Agent Registration**: Multiple agents are registered using `builder.AddAIAgent()` with custom additional properties
2. **AgentCatalog Usage**: The `AgentCatalog` service is injected into the endpoint handler
3. **Metadata Extraction**: Additional properties are extracted from each agent and mapped to a response model
4. **Type Safety**: Uses strongly-typed classes and enums for the API response

## Key Concepts

- **AgentCatalog**: Provides enumeration of all registered agents in the application
- **AdditionalProperties**: A dictionary that can store custom metadata on agents
- **Minimal APIs**: Uses ASP.NET Core minimal API syntax for clean, concise endpoint definitions
- **Dependency Injection**: Agents are registered in DI and resolved through the `AgentCatalog`
