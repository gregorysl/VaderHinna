using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VaderHinna.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;
        private IConfiguration Configuration { get; }

        public DevicesController(ILogger<DevicesController> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        [Route("{deviceId}/[action]/{date}/{sensor?}")]
        public string Data(string deviceId, string date, string sensor)
        {
            var (isValid, errorMessage) = ParametersValidator(deviceId, date, sensor);
            if (!isValid)
            {
                return errorMessage;
            }
            return $"Hello World!{deviceId} {date} {sensor}";
        }

        private KeyValuePair<bool, string> ParametersValidator(string deviceId, string dateString, string sensor)
        {
            var isValidDate = DateTime.TryParseExact(dateString, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date);
            if (!isValidDate) return new KeyValuePair<bool, string>(false, "Date is not in correct format");
            if (date > DateTime.Today) return new KeyValuePair<bool, string>(false, "Date cannot be set in future");

            var azureCache = new AzureConnector(Configuration).DiscoveryMode().Result;
            var isValidDevice = azureCache.Devices.Any(x => x.Id == deviceId);
            if (!isValidDevice) return new KeyValuePair<bool, string>(false, "Unknown device Id");

            var isValidSensor = string.IsNullOrEmpty(sensor) ||
                                 azureCache.Devices.Single(x => x.Id == deviceId).Sensors.Any(x => x == sensor);
            if (!isValidSensor) return new KeyValuePair<bool, string>(false, "Sensor not recognized for this device");

            return new KeyValuePair<bool, string>(true, null);
        }
    }
}