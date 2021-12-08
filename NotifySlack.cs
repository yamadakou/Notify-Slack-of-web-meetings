using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using dcinc.api.entities;
using dcinc.api.queries;
using Microsoft.Azure.Documents.Client;
using dcinc.api;

namespace dcinc.jobs
{
    public static class NotifySlack
    {
        [FunctionName("NotifySlack")]
        public static void Run([TimerTrigger("0 0 9 * * 1-5")]TimerInfo myTimer, 
        [CosmosDB(
                databaseName: "Notify-Slack-of-web-meetings-db",
                collectionName: "WebMeetings",
                ConnectionStringSetting = "CosmosDbConnectionString")
                ]DocumentClient client,
        ILogger log)
        {
            var today = DateTime.UtcNow.ToString("YYYY-MM-DD");
            var webMeetingsParam = new WebMeetingsQueryParameter()
            {
                FromDate = today,
                ToDate = today
            };

            var webMeeting = webMeetings.GetWebMeetings(client, webMeetingsParam, log);
            //var slackChannel = SlackChannels.GetSlackChannels();
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
