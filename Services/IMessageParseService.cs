using TradeApi.Models;

namespace TradeApi.Services
{
    public interface IMessageParseService
    {
        GoldCupTradeData ParseGCTradeData(string message);
    }
}