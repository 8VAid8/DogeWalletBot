using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DogeWalletBot.Model
{
    [Serializable]
    public class BalanceEntity
    {
        public string Balance { get; set; }
        public int Success { get; set; }
    }
}