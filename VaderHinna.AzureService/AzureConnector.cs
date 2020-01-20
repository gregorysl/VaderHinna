using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VaderHinna.Model;

namespace VaderHinna.AzureService
{
    public class AzureConnector : IAzureConnector
    {
        private string _name = "iotbackend";
        private string _filename = "metadata.csv";
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly Uri _baseUri;
        public AzureConnector(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _baseUri = storageAccount.BlobStorageUri.PrimaryUri;
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }
        
        public async Task<AzureCache> DiscoveryMode()
        {
            var list = await GetFilesFromDirectory(_cloudBlobClient, _name);
            var files = list.Results.Where(x => x.GetType() == typeof(CloudBlockBlob))
                .Cast<CloudBlockBlob>()
                .Select(x => new AzureFile { Length = x.StreamWriteSizeInBytes, Name = x.Name, Uri = x.Uri })
                .ToList();

            if (files.Any(x => x.Name == _filename))
            {
                var file = files.Single(x => x.Name == _filename);
                var cache = await CreateCache(file);
                return cache;
            }

            return null;
        }
        
        private async Task<AzureCache> CreateCache(AzureFile file)
        {
            var converted = await DownloadTextByBlobUri(file.Uri);
            using var csv = new CsvReader(new StringReader(converted), CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.Delimiter = ";";
            var devicesList = csv.GetRecords<DeviceSensorData>()
                .GroupBy(x => x.DeviceId)
                .Select(x => new AzureDevice { Id = x.Key, Sensors = x.Select(z => z.Sensor).ToList() }).ToList();
            return new AzureCache { File = file, Devices = devicesList };
        }

        private static async Task<BlobResultSegment> GetFilesFromDirectory(CloudBlobClient cloudBlobClient, string name)
        {
            var backupContainer = cloudBlobClient.GetContainerReference(name);
            var list = await backupContainer.ListBlobsSegmentedAsync(null);
            return list;
        }
        
        public async Task<string> DownloadTextByBlobUri(Uri uri)
        {
            var blob = new CloudBlockBlob(uri);
            var content = await blob.DownloadTextAsync();
            return content;
        }
        public async Task<string> DownloadTextByAppendUri(Uri uri)
        {
            var blob = new CloudAppendBlob(uri);
            var content = await blob.DownloadTextAsync();
            return content;
        }

        public async Task<List<DeviceData>> DownloadDeviceDataForSensor(Uri uri)
        {
            var data = await DownloadTextByAppendUri(uri);
            using var csv = new CsvReader(new StringReader(data), CultureInfo.InvariantCulture);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.RegisterClassMap<FooMap>();
            csv.Configuration.Delimiter = ";";
            var devicesList = csv.GetRecords<DeviceData>().ToList();
            return devicesList;
        }

        public async Task<bool> CheckBlobUrlExist(Uri uri)
        {
            var cloudBlob = new CloudBlob(uri);
            return await cloudBlob.ExistsAsync();
        }
    }
    public class DeviceSensorData
    {
        [Index(0)]
        public string DeviceId { get; set; }

        [Index(1)]
        public string Sensor { get; set; }
    }
    
    public class DeviceData
    {
        [Index(0)]
        public DateTime Date { get; set; }

        [Index(1)]
        public float Value { get; set; }
    }
    public sealed class FooMap : ClassMap<DeviceData>
    {
        public FooMap()
        {
            Map(m => m.Date).Index(0);
            Map(m => m.Value).TypeConverterOption.CultureInfo(new CultureInfo("pl-PL"));
        }
    }
}
