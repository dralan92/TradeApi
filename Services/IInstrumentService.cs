using TradeApi.Models;

namespace TradeApi.Services
{
    public interface IInstrumentService
    {
        InstrumentList GetInstrumentList();
    }
}