using System;
using System.Collections.Generic;

namespace VaderHinna.Model
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

    public class AzureDevice
    {
        public string Id { get; set; }
        public List<string> Sensors { get; set; }
    }
    public class AzureCache
    {
        public string BaseUrl { get; set; }
        public AzureFile File { get; set; }
        public List<AzureDevice> Devices { get; set; }
    }
    
    public class MetadataInfo
    {
        public string DeviceId { get; set; }
        public string Sensor { get; set; }
    }
    
    public class SensorData
    {
        public DateTime Date { get; set; }
        public float Value { get; set; }
    }
}
