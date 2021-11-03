using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using dcinc.api.entities;
using FluentValidation;

namespace dcinc.api
{
    public static class webMeetings
    {
        [FunctionName("webMeetings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "Notify-Slack-of-web-meetings-db",
                collectionName: "WebMeetings",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string message = string.Empty;           
            
            switch (req.Method)
            {
                case "GET":
                    log.LogInformation("GET webMeetings");
                    break;
                case "POST":
                    log.LogInformation("POST webMeetings");
                    
                    // リクエストのBODYからパラメータ取得
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);
                    
                    var webMeeting = new WebMeeting();
                    webMeeting.Name = data?.name;
                    webMeeting.StartDateTime = data?.startDateTime;
                    webMeeting.Url = data?.meetingUrl;
                    webMeeting.RegisteredAt = data?.registeredAt;
                    webMeeting.SlackChannelId = data?.slackChannelId;

                    // 入力値チェックを行う
                    var webMeetingValidator = new WebMeetingValidator();
                    webMeetingValidator.ValidateAndThrow(webMeeting);
                    
                    // Web会議情報を登録
                    message = await AddWebMeetings(documentsOut, webMeeting);
                    break;

                default:
                    throw new InvalidOperationException($"Invalid method: method={req.Method}");
            }   

            return new OkObjectResult($"This HTTP triggered function executed successfully.\n{message}");
        }

        private static async Task<string> AddWebMeetings(
            IAsyncCollector<dynamic> documentsOut, WebMeeting webMeeting)
        {
            // Add a JSON document to the output container.
            var documentItem = new
            {
                webMeeting
            };

            await documentsOut.AddAsync(documentItem);
            return JsonConvert.SerializeObject(documentItem);
        }
    }
}
