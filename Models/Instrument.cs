using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradeApi.Models
{
    public class Instrument
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string PipLocation { get; set; }
    }
}
