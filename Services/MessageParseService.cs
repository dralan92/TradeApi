using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeApi.Models;

namespace TradeApi.Services
{
    public class MessageParseService : IMessageParseService
    {
        IInstrumentService _instrumentService;
        public MessageParseService(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }
        public GoldCupTradeData ParseGCTradeData(string message)
        {

            var messageupper = message.ToUpper();
            var indexTp = AllIndexOf(messageupper.ToUpper(), "TP");
            var indexSL = AllIndexOf(messageupper.ToUpper(), "SL");
            var tp1Price = ExtractPrice(indexTp[0] + 3, messageupper);
            var tp2Price = ExtractPrice(indexTp[1] + 3, messageupper);
            var slPrice = ExtractPrice(indexSL[0] + 2, messageupper);
            var entry = ExtractPrice(0, RemoveSubString(slPrice.ToString(),
                            RemoveSubString(tp2Price.ToString(),
                            RemoveSubString(tp1Price.ToString(), messageupper))));
            var instrument = ParseInstrument(messageupper).Name;

            var action = messageupper.IndexOf("SELL") > -1 ? "SELL" : "BUY";

            var gctd = new GoldCupTradeData{
                Action = action,
                Entry = entry,
                InstrumentName = instrument,
                SL = slPrice,
                TP = tp1Price
            };
            return gctd;
        }


        List<int> AllIndexOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
        double ExtractPrice(int startingIndex, string input)
        {
            var cArray = input.ToCharArray();
            var priceStarted = false;
            var resultString = "";
            for (var i = startingIndex; i < cArray.Length - 1; i++)
            {
                if (Char.IsDigit(cArray[i]) || cArray[i] == '.')
                {
                    priceStarted = true;
                    resultString += cArray[i].ToString();
                }
                else
                {
                    if (priceStarted) break;
                    continue;
                }
            }
            var parseResult = Double.TryParse(resultString, out double price);
            return price;
        }
        string RemoveSubString(string subString, string input)
        {
            var index = input.IndexOf(subString);
            return (index < 0)
                        ? input
                        : input.Remove(index, subString.Length);
        }

        Instrument ParseInstrument(string input)
        {
            var instrumentList = _instrumentService.GetInstrumentList();
            foreach (var instrument in instrumentList.Instruments)
            {
                var index = input.IndexOf(instrument.Name.Replace("_","/"));
                if (index > 0)
                {
                    return instrument;
                }
            }
            return new Instrument();
        }
    }
}
