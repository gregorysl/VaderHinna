using System.Collections.Generic;

namespace VaderHinna.Model.Interfaces
{
    public interface ICsvService
    {
        List<SensorData> ReadAndParseSensorData(string data);
        List<AzureDevice> ParseMetadataInfoForDevices(string converted);
    }
}
