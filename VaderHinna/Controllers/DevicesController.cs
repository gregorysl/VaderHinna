using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VaderHinna.Model;
using VaderHinna.Model.Interface;

namespace VaderHinna.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly MemoryCacheEntryOptions _cacheOption = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DateTime.Now.AddMinutes(30) - DateTime.Now
        };
        private readonly ILogger<DevicesController> _logger;
        private readonly ICsvService _csvService;
        private IAzureConnector Connector { get; }
        private readonly IMemoryCache _memoryCache;
        private string CACHE_KEY = "AzureCache";

        public List<AzureDevice> DeviceList
        {
            get
            {
                if (_memoryCache.Get(CACHE_KEY) != null)
                {
                    return (List<AzureDevice>)_memoryCache.Get(CACHE_KEY);
                }

                var azureCache = Connector.DeviceDiscovery().Result;
                _memoryCache.Set(CACHE_KEY, azureCache, _cacheOption);
                return azureCache;
            }
        }

        public DevicesController(ILogger<DevicesController> logger, IAzureConnector connector, IMemoryCache memoryCache, ICsvService csvService)
        {
            Connector = connector;
            _logger = logger;
            _memoryCache = memoryCache;
            _csvService = csvService;

        }

        [HttpGet]
        [Route("{deviceId}/[action]/{date}/{sensor?}")]
        public async Task<IActionResult> Data(string deviceId, string date, string sensor)
        {
            var errorMessage = ParametersValidator(deviceId, date, sensor);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _logger.LogError(errorMessage, deviceId, date, sensor);
                return BadRequest(errorMessage);
            }

            var sensorsToDownload = DeviceList
                .Single(x => x.Id == deviceId).Sensors
                .Where(x => string.IsNullOrEmpty(sensor) || x == sensor).ToList();
            var result = new Dictionary<string, List<SensorData>>();
            foreach (var sensorName in sensorsToDownload)
            {
                var newUri = Connector.CreateUrl($"{deviceId}/{sensorName}/{date}.csv");
                var uri = new Uri(newUri);
                if (!await Connector.BlobForUrlExist(uri))
                {
                    const string error = "Requested data doesn't exist";
                    _logger.LogError(error, newUri);
                    return NotFound(error);
                }
                var dataForSensor = await DownloadDeviceDataForSensor(uri);
                result.Add(sensorName, dataForSensor);
            }

            return Ok(result);
        }

        private string ParametersValidator(string deviceId, string dateString, string sensor)
        {
            var isValidDate = DateTime.TryParseExact(dateString, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date);
            if (!isValidDate) return "Date is not in correct format";
            if (date > DateTime.Today) return "Date cannot be set in future";

            var isValidDevice = DeviceList.Any(x => x.Id == deviceId);
            if (!isValidDevice) return "Unknown device Id";

            var isValidSensor = string.IsNullOrEmpty(sensor) ||
                                DeviceList.Single(x => x.Id == deviceId).Sensors.Any(x => x == sensor);
            if (!isValidSensor) return "Sensor not recognized for this device";

            return null;
        }

        public async Task<List<SensorData>> DownloadDeviceDataForSensor(Uri uri)
        {
            var data = await Connector.DownloadTextByAppendUri(uri);
            var devicesList = _csvService.ReadAndParseSensorData(data);
            return devicesList;
        }

    }
}