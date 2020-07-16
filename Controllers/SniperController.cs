using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using TradeApi.Models;
using TradeApi.Services;

namespace TradeApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SniperController : ControllerBase
    {
      
        IPriceService _priceService;

        public SniperController( IPriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpGet]
        public string Get([FromQuery]string instrument, [FromQuery]string action)
        {
            var isLong = action == "BUY" ? true : false;
            var price = _priceService.GetPriceList(instrument).Prices[0];
            var units = CalculateUnits(price);
            if (units >= 1)
            {
                var ask = GetAsk(price);
                var bid = GetBid(price);
                var takeProfit = CalculateTakeProfit(ask, bid, isLong);
                var stopLoss = CalculateStopLoss(ask, bid, isLong);
                if (!isLong) { units *= -1; }
                PlaceRealOrder(instrument, takeProfit, stopLoss, units);
            }
            return "request posted" + instrument + action;
        }
        void PlaceRealOrder(string instrument, double tp, double sl, int units)
        {
            var jsonBody = "{\"order\": {\r\n    \"units\": \"" +
                units +
                "\",\r\n    \"instrument\": \"" +
               instrument +
                "\",\r\n    \"timeInForce\": \"FOK\",\r\n    \"type\": \"MARKET\",\r\n    \"takeProfitOnFill\":{\"price\":\"" +
                tp +
                "\"},\r\n    \"stopLossOnFill\" :{\"price\":\"" +
                sl +
                "\"},\r\n    \"positionFill\": \"DEFAULT\"\r\n  }\r\n}";
            var restClient = new RestClient("https://api-fxpractice.oanda.com/v3/accounts/101-002-14835452-002/orders");
            var restRequest = new RestRequest();
            restRequest.Method = RestSharp.Method.POST;


            restRequest.AddHeader("Authorization", "Bearer 551cb19f4a836f950717a0202b8dde87-adad3a86ed84c81861f9b947c435220c");
            restRequest.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            var response = restClient.Execute(restRequest);
        }
        int CalculateUnits(Price price)
        {
            int availableUnits;
            var parseUnit = int.TryParse(price.UnitsAvailable.Default.Long, out availableUnits);
            var unitsForTrade = (int)Math.Floor(availableUnits * .02);
            if (unitsForTrade >= 1)
            {
                return unitsForTrade;
            }

            return -1;
        }
        double GetAsk(Price price)
        {
            var ask = 0.0;
            var parseAsk = double.TryParse(price.CloseOutAsk, out ask);
            return ask;
        }
        double GetBid(Price price)
        {
            var bid = 0.0;
            var parseBid = double.TryParse(price.CloseOutBid, out bid);
            return bid;
        }
        double CalculateTakeProfit(double ask, double bid, bool isLong)
        {
            var spread = ask - bid;
            var tp = isLong ? ask + (4 * spread) : bid - (4 * spread);
            var dp = GetDecimalPrecision(ask);
            return Math.Round(tp,dp);
        }
        double CalculateStopLoss(double ask, double bid, bool isLong)
        {
            var spread = ask - bid;
            var sl = isLong ? ask - (2 * spread) : bid + (2 * spread);
            var dp = GetDecimalPrecision(ask);
            return Math.Round(sl,dp);
        }

        int GetDecimalPrecision(double input)
        {
            var parts = input.ToString().Split('.');
            var numberOfDecimalPlaces = parts[1].Count();
            return numberOfDecimalPlaces;
        }

    }
}
