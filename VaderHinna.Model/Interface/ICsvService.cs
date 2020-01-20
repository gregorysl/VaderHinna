using System.Collections.Generic;
using System.IO;

namespace VaderHinna.Model.Interface
{
    public interface ICsvService
    {
        List<SensorData> ReadAndParseSensorData(string data);
        List<AzureDevice> ParseMetadataInfoFromStream(Stream stream);
    }
}
