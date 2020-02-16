using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using VaderHinna.Model;
using VaderHinna.Model.Interface;

namespace VaderHinna.AzureService
{
    public class CsvService: ICsvService
    {
        public List<SensorData> ReadAndParseSensorData(string data)
        {
            using var streamReader = new StreamReader(data);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.Delimiter = ";";
            csv.Configuration.RegisterClassMap<SensorDataMap>();
            var devicesList = csv.GetRecords<SensorData>().ToList();
            return devicesList;
        }

        public List<AzureDevice> ParseMetadataInfoFromStream(Stream stream)
        {
            stream.Position = 0;
            using var streamReader = new StreamReader(stream);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.Delimiter = ";";
            csv.Configuration.RegisterClassMap<MetadataInfoMap>();
            var devicesList = csv.GetRecords<MetadataInfo>()
                .GroupBy(x => x.DeviceId)
                .Select(x => new AzureDevice {Id = x.Key, Sensors = x.Select(z => z.Sensor).ToList()}).ToList();
            return devicesList;
        }
    }
    public sealed class SensorDataMap : ClassMap<SensorData>
    {
        public SensorDataMap()
        {
            Map(m => m.Date).Index(0);
            Map(m => m.Value).Index(1).TypeConverterOption.CultureInfo(new CultureInfo("pl-PL"));
        }
    }
    public sealed class MetadataInfoMap : ClassMap<MetadataInfo>
    {
        public MetadataInfoMap()
        {
            Map(m => m.DeviceId).Index(0);
            Map(m => m.Sensor).Index(1);
        }
    }
}
