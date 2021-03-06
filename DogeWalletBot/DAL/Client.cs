﻿
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
        public static string WebApiHost { get; set; } = "http://dogechain.info/api/v1/";

        public static string WebApiHostDogechain { get; set; } = "http://dogechain.info/api/v1/";
        public static string WebApiHostChainSo { get; set; } = "https://chain.so/api/v2/";

        public static string WebApiHostCoinmarket { get; set; } = "https://api.coinmarketcap.com/v1/";

        public static void InitClient() => InitClient(WebApiHost);
        public static void InitClient(string webApiHost)
        {
            WebApiHost = webApiHost;
            if (client != null && !string.IsNullOrEmpty(WebApiHost) && client.BaseAddress.AbsoluteUri == WebApiHost)
                return;
            client = new HttpClient
            {
                BaseAddress = new Uri(WebApiHost)
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

        public static async Task<byte[]> GetByteArrayAsync(string path)
        {
            InitClient();
            byte[] byteArray;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                byteArray = await response.Content.ReadAsByteArrayAsync();
                return byteArray;
            }
            throw new Exception("Can't receive qr code!");
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

        public static async Task<string> GetQRCode(string address)
        {
            string qrPath = $"address/qrcode/{address}";
            var img = await GetByteArrayAsync(qrPath);
            if (img != null)
                return WebApiHost + qrPath;
            else
                return null;
        }

        public static async Task<List<ReceivedTransaction>> GetReceivedTransactions(string address)
        {
            WebApiHost = WebApiHostChainSo;
            var trs = await GetAsync<ReceivedTransactionsResponse>($"get_tx_received/DOGE/{address}");
            WebApiHost = WebApiHostDogechain;
            return trs?.Data?.Txs;
        }

        public static async Task<List<ExchangeRate>> GetExchangeRate()
        {
            WebApiHost = WebApiHostCoinmarket;
            var rate = await GetAsync<List<ExchangeRate>>($"ticker/dogecoin/");
            WebApiHost = WebApiHostDogechain;
            return rate;
        }
    }
}