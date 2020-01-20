using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VaderHinna.Model;
using VaderHinna.Model.Interfaces;

namespace VaderHinna.AzureService
{
    public class AzureConnector : IAzureConnector
    {
        private string _name = "iotbackend";
        private string _filename = "metadata.csv";
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly ICsvService _csvService;
        private readonly string _baseUrl;
        public AzureConnector(string connectionString, ICsvService csvService)
        {
            _csvService = csvService;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _baseUrl = $"{storageAccount.BlobStorageUri.PrimaryUri}{_name}";
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
            var devicesList = _csvService.ParseMetadataInfoForDevices(converted);
            return new AzureCache { BaseUrl = _baseUrl, File = file, Devices = devicesList };
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

        public async Task<List<SensorData>> DownloadDeviceDataForSensor(Uri uri)
        {
            var data = await DownloadTextByAppendUri(uri);
            var devicesList = _csvService.ReadAndParseSensorData(data);
            return devicesList;
        }

        public async Task<bool> CheckBlobUrlExist(Uri uri)
        {
            var cloudBlob = new CloudBlob(uri);
            return await cloudBlob.ExistsAsync();
        }
    }
}
