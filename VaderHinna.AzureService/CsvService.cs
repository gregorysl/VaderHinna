using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using VaderHinna.Model;
using VaderHinna.Model.Interfaces;

namespace VaderHinna.AzureService
{
    public class CsvService: ICsvService
    {
        public List<SensorData> ReadAndParseSensorData(string data)
        {
            using var csv = new CsvReader(new StringReader(data), CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.RegisterClassMap<SensorDataMap>();
            csv.Configuration.Delimiter = ";";
            var devicesList = csv.GetRecords<SensorData>().ToList();
            return devicesList;
        }

        public List<AzureDevice> ParseMetadataInfoForDevices(string converted)
        {
            using var csv = new CsvReader(new StringReader(converted), CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.Delimiter = ";";
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
}
