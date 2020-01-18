using System;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

namespace VaderHinna
{
    public class AzureFile
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public Uri Uri { get; set; }
    }
    public class AzureDir
    {
        public string Path { get; set; }
        public List<string> Files { get; set; }
    }
    public class DeviceSensorData
    {
        [Index(0)]
        public string DeviceId { get; set; }

        [Index(1)]
        public string Sensor { get; set; }
    }

    public class AzureDevice
    {
        public string Id { get; set; }
        public List<string> Sensors { get; set; }
    }
    public class AzureCache
    {
        public AzureFile File { get; set; }
        public List<AzureDevice> Devices { get; set; }
    }
}
