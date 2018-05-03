using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DogeWalletBot.Model
{
    public class ReceivedTransaction
    {
        public string Txid { get; set; }
        public string Value { get; set; }
        public string Confirmations { get; set; }
    }

    public class ReceivedTransactionsEntity
    {
        public string Address { get; set; }
        public List<ReceivedTransaction> Txs { get; set; }
    }

    public class ReceivedTransactionsResponse
    {
        public string Status { get; set; }
        public ReceivedTransactionsEntity Data { get; set; }
    }
}