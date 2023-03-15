using System;
using Newtonsoft.Json;

namespace dcinc.api.entities
{
    /// <summary>
    /// ユーザーを表す
    /// </summary>
    public class User
    {
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 一意とするID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Eメールアドレス
        /// </summary>
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 認可キー
        /// </summary>
        /// <value></value>
        [JsonProperty("authorizationKey")]
        public string AuthorizationKey { get; set; }
    }
}