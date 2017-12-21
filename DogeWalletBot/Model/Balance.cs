using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DogeWalletBot.Model
{
    [Serializable]
    public class BalanceEntity
    {
        public string balance { get; set; }
        public int success { get; set; }
    }
}