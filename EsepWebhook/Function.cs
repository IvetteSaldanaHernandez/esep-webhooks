using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)
)]

namespace EsepWebhook;

public class Function
{
    private static readonly HttpClient Client = new HttpClient();

    public async Task<string> FunctionHandler(
        object input,
        ILambdaContext context)
    {
        context.Logger.LogLine($"Received event: {input}");

        // Parse the incoming GitHub webhook payload.
        var json = JObject.Parse(input.ToString() ?? "{}");

        // Get the URL of the GitHub issue.
        var issueUrl = json["issue"]?["html_url"]?.ToString();

        // Ignore events that do not contain an issue URL.
        if (string.IsNullOrWhiteSpace(issueUrl))
        {
            context.Logger.LogLine("No issue URL found.");
            return "Ignored event";
        }

        // Only send notifications when an issue is opened.
        var action = json["action"]?.ToString();

        if (action != null && action != "opened")
        {
            context.Logger.LogLine($"Ignoring action: {action}");
            return "Ignored event";
        }

        // Retrieve the Slack webhook URL from AWS environment variables.
        var slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");

        if (string.IsNullOrWhiteSpace(slackUrl))
        {
            throw new InvalidOperationException(
                "SLACK_URL environment variable is missing."
            );
        }

        // Prepare the Slack notification.
        var payload = JsonConvert.SerializeObject(new
        {
            text = $"Issue Created: {issueUrl}"
        });

        // Send the notification to Slack.
        using var content = new StringContent(
            payload,
            Encoding.UTF8,
            "application/json"
        );

        using var response = await Client.PostAsync(slackUrl, content);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        context.Logger.LogLine("Slack notification sent successfully.");

        return result;
    }
}
