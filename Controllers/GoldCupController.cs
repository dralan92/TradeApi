using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradeApi.Services;

namespace TradeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoldCupController : ControllerBase
    {
        IMessageParseService _messageParseService;
        public GoldCupController(IMessageParseService messageParseService)
        {
            _messageParseService = messageParseService;
        }
        [HttpGet]
        public string Get([FromQuery]string message)
        {
            var gctd = _messageParseService.ParseGCTradeData(message);
            return "Hiii";
        }
    }
}