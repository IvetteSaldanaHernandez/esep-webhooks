using System;
using System.Threading.Tasks;
using Amazon.Lambda.TestUtilities;
using EsepWebhook;
using Xunit;

namespace EsepWebhook.Tests;

public class FunctionTests
{
    [Fact]
    public async Task IgnoresEventsWithoutIssueUrl()
    {
        var function = new Function();
        var context = new TestLambdaContext();

        // Simulate a GitHub ping event.
        string input = """
        {
            "zen": "GitHub webhook ping"
        }
        """;

        var result = await function.FunctionHandler(input, context);

        Assert.Equal("Ignored event", result);
    }

    [Fact]
    public async Task IgnoresClosedIssues()
    {
        var function = new Function();
        var context = new TestLambdaContext();

        // Simulate a GitHub issue being closed.
        string input = """
        {
            "action": "closed",
            "issue": {
                "html_url": "https://github.com/example/repo/issues/1"
            }
        }
        """;

        var result = await function.FunctionHandler(input, context);

        Assert.Equal("Ignored event", result);
    }

    [Fact]
    public async Task RequiresSlackUrlForOpenedIssues()
    {
        var function = new Function();
        var context = new TestLambdaContext();

        // Save the original environment variable.
        var originalUrl = Environment.GetEnvironmentVariable("SLACK_URL");

        try
        {
            // Temporarily remove the Slack webhook URL.
            Environment.SetEnvironmentVariable("SLACK_URL", null);

            // Simulate a newly opened GitHub issue.
            string input = """
            {
                "action": "opened",
                "issue": {
                    "html_url": "https://github.com/example/repo/issues/2"
                }
            }
            """;

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => function.FunctionHandler(input, context)
            );
        }
        finally
        {
            // Restore the original environment variable.
            Environment.SetEnvironmentVariable("SLACK_URL", originalUrl);
        }
    }
}
