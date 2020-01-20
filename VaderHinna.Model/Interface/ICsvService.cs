using System.Collections.Generic;

namespace VaderHinna.Model.Interface
{
    public interface ICsvService
    {
        List<SensorData> ReadAndParseSensorData(string data);
        List<AzureDevice> ParseMetadataInfoForDevices(string converted);
    }
}
