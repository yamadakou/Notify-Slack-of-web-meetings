using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Microsoft.AspNetCore.Http;
using dcinc.api.queries;
using System.Threading.Tasks;
using System.Linq;

namespace dcinc.api
{
    public static class AuthorizationUtility
    {
        /// <summary>
        /// 認可できるかを確認する
        /// </summary>
        /// <param name="client"></param>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task<bool> CheckAuthorization(DocumentClient client, HttpRequest req, ILogger log)
        {
            var email = req.Headers["x-nsw-email-address"];
            var authKey = req.Headers["x-nsw-auth-key"];

            // 認可情報が無い場合は認可しない
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(authKey)) return false;

            // クエリパラメータから検索条件パラメータを設定
            UsersQueryParameter getUserqueryParameter = new UsersQueryParameter()
            {
                EmailAddress = email,
                AuthorizationKey = authKey
            };
            
            var users = await Users.GetUsersAsync(client, getUserqueryParameter,log);
            
            // 該当のユーザーがいなければfalseを返す
            if(!users.Any()) return false;

            return true;
        }
    }
}