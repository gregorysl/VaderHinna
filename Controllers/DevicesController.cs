using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VaderHinna.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ILogger<DevicesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{deviceId}/data/{date}/{sensor}")]
        public string Get(string deviceId, string date, string sensor)
        {
            return $"Hello World!{deviceId} {date} {sensor}";
        }
    }
}