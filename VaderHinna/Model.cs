using CsvHelper.Configuration.Attributes;

namespace VaderHinna
{

    public class DeviceSensorData
    {
        [Index(0)]
        public string DeviceId { get; set; }

        [Index(1)]
        public string Sensor { get; set; }
    }
}
