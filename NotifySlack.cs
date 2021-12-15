using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using dcinc.api.entities;
using dcinc.api.queries;
using Microsoft.Azure.Documents.Client;
using dcinc.api;
using System.Linq;

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
            // 原罪日のWeb会議情報を取得する
            var today = DateTime.UtcNow.ToString("YYYY-MM-DD");
            var webMeetingsParam = new WebMeetingsQueryParameter()
            {
                FromDate = today,
                ToDate = today
            };

            // 取得したWeb会議情報のSlackチャンネルを取得する
            var webMeetings = WebMeetings.GetWebMeetings(client, webMeetingsParam, log);
            var slackChannelIds = webMeetings.Result.Select(webMeeting => webMeeting.SlackChannelId).Distinct();
            var slackChannelParams = new SlackChannelsQueryParameter{
                Ids = string.Join(", ", slackChannelIds)
            };
            var slackChannels = SlackChannels.GetSlackChannels(client, slackChannelParams, log);
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        
            // Slackに通知する
        }
    }
}
