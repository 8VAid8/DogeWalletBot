
using DogeWalletBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Budget.Bot.DAL
{
    /// <summary>
    /// Represents CRUD operations with entities
    /// </summary>
    public static class Client
    {
        public static HttpClient client; //don't create too much client instanses, one is enough
        static string webApiHost = "http://dogechain.info/api/v1/";

        public static void InitClient()
        {
            if (client != null && !string.IsNullOrEmpty(webApiHost))
                return;
            client = new HttpClient
            {
                BaseAddress = new Uri(webApiHost)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<T> GetAsync<T>(string path)
        {
            InitClient();
            T entity = default(T);
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                entity = JsonConvert.DeserializeObject<T>(jsonString);
            }
            return entity;
        }

        public static async Task<BalanceEntity> GetBalanceAsync(string address)
        {
            return await GetAsync<BalanceEntity>($"address/balance/{address}");
        }

        public static async Task<ReceivedEntity> GetReceivedAsync(string address)
        {
            return await GetAsync<ReceivedEntity>($"address/received/{address}");
        }

        public static async Task<SentEntity> GetSentAsync(string address)
        {
            return await GetAsync<SentEntity>($"address/sent/{address}");
        }
    }
}