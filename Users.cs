using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Collections.Generic;
using dcinc.api.entities;
using dcinc.api.queries;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using FluentValidation;
using Newtonsoft.Json.Serialization;
using Microsoft.Azure.Documents;
using User = dcinc.api.entities.User;

namespace dcinc.api
{
    public static class Users
    {
        #region ユーザーの取得

        /// <summary>
        /// ユーザーを取得する。
        /// </summary>
        /// <param name="client">CosmosDBのドキュメントクライアント</param>
        /// <param name="queryParameter">抽出条件パラメータ</param>
        /// <param name="log">ロガー</param>
        /// <returns>ユーザー一覧</returns>
        internal static async Task<IEnumerable<User>> GetUsersAsync(
            DocumentClient client,
            UsersQueryParameter queryParameter,
            ILogger log)
        {
            // Get a JSON document from the container.
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("Notify-Slack-of-web-meetings-db", "Users");
            IDocumentQuery<User> query = client.CreateDocumentQuery<User>(collectionUri, new FeedOptions { EnableCrossPartitionQuery = true, PopulateQueryMetrics = true })
                .Where(queryParameter.GetWhereExpression())
                .AsDocumentQuery();
            log.LogInformation(query.ToString());

            var documentItems = new List<User>();
            while (query.HasMoreResults)
            {
                foreach (var documentItem in await query.ExecuteNextAsync<User>())
                {
                    documentItems.Add(documentItem);
                }
            }
            
            return documentItems;
        }

        #endregion
    }
}