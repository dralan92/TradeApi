using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradeApi.Models
{
    public class Price
    {
        public string CloseOutBid { get; set; }
        public string CloseOutAsk { get; set; }
        public UnitsAvailable UnitsAvailable { get; set; }
        public string Instrument { get; set; }
    }
}
