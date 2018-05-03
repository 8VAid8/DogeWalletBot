using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DogeWalletBot.Model
{
    public class ExchangeRate
    {
        public string Name { get; set; }
        public string Price_usd { get; set; }
        public string Price_btc { get; set; }
    }
}