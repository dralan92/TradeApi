using TradeApi.Models;

namespace TradeApi.Services
{
    public interface IPriceService
    {
        PriceList GetPriceList(string instrumentName);
    }
}