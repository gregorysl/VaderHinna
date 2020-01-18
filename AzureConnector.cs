using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace VaderHinna
{
    public class AzureConnector
    {
        private string _name = "iotbackend";
        private string _filename = "metadata.csv";
        private readonly CloudBlobClient _cloudBlobClient;
        public AzureConnector(IConfiguration configuration)
        {
            Configuration = configuration;
            
            var connectionstring = Configuration["ConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionstring);

            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        public IConfiguration Configuration { get; }

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
        private static async Task<AzureCache> CreateCache(AzureFile file)
        {
            var converted = await DownloadStringByBlobName(file.Uri);
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

        private static async Task<string> DownloadStringByBlobName(Uri uri)
        {
            var blob = new CloudBlockBlob(uri);
            var content = await blob.DownloadTextAsync();
            return content;
        }
    }
}
