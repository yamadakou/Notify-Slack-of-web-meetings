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
    public static class SlackChannels
    {
        [FunctionName("SlackChannels")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
                        [CosmosDB(
                databaseName: "Notify-Slack-of-web-meetings-db",
                collectionName: "SlackChannels",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string message = string.Empty;           
            try
            {
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
                        
                        var slackChannels = new SlackChannel();
                        slackChannels.Name = data?.name;
                        slackChannels.WebhookUrl = data?.webhookUrl;
                        slackChannels.RegisteredBy = data?.registeredBy;

                        // 入力値チェックを行う
                        var slackChannelValidator = new SlackChannelValidator();
                        slackChannelValidator.ValidateAndThrow(slackChannels);
                        
                        // slackチャンネル情報を登録
                        message = await AddSlackChannels(documentsOut, slackChannels);
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid method: method={req.Method}");
                }   
            }
            catch(Exception ex) 
            {
                return new BadRequestObjectResult(ex);
            }

            
            return new OkObjectResult($"This HTTP triggered function executed successfully.\n{message}");
        }

        /// <summary>
        /// Slackチャンネル情報を登録する
        /// </summary>
        /// <param name="documentsOut">CosmosDBのドキュメント</param>
        /// <param name="slackChannel">Slackチャンネルの情報</param>
        /// <returns></returns>
        private static async Task<string> AddSlackChannels(
            IAsyncCollector<dynamic> documentsOut, SlackChannel slackChannel)
        {
            // Add a JSON document to the output container.
            var documentItem = new
            {
                slackChannel
            };

            await documentsOut.AddAsync(documentItem);
            return JsonConvert.SerializeObject(documentItem);
        }
    }
}
