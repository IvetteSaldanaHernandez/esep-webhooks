# ESEP Webhooks — Event-Driven Architecture

## Overview

This project demonstrates an event-driven architecture that connects GitHub issues to Slack notifications using AWS Lambda and API Gateway.

When a new GitHub issue is created, GitHub sends a webhook event to an API Gateway endpoint. API Gateway forwards the request to an AWS Lambda function, which extracts the issue URL and sends a notification to Slack.

## Technologies

- C# / .NET 10
- AWS Lambda
- Amazon API Gateway
- GitHub Webhooks
- Slack Incoming Webhooks
- Newtonsoft.Json
- xUnit

## How It Works

1. A user creates a GitHub issue.
2. GitHub sends an HTTP POST request containing the issue information.
3. API Gateway forwards the request to the Lambda function.
4. The function extracts the issue URL and checks the event type.
5. The function sends a Slack notification using the `SLACK_URL` environment variable.

## Local Development

Build the Lambda project:

```bash
dotnet build EsepWebhook/EsepWebhook.csproj
```

Run the unit tests:

```bash
dotnet test EsepWebhook.Tests/EsepWebhook.Tests.csproj
```

## AWS Configuration

The Lambda function requires an environment variable named `SLACK_URL` containing the Slack incoming webhook URL.

This value must be configured securely in AWS and should never be committed to GitHub.

## Current Status

The Lambda function has been implemented locally, and automated tests have been added to verify webhook event handling.

AWS deployment is currently blocked because my AWS account was suspended. As a result, the API Gateway integration and end-to-end GitHub-to-Slack notification flow have not yet been verified.

The project is ready for deployment once AWS access is restored.