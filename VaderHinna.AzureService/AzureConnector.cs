using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VaderHinna.Model;
using VaderHinna.Model.Interface;

namespace VaderHinna.AzureService
{
    public class AzureConnector : IAzureConnector
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly ICsvService _csvService;
        private readonly string _baseUrl;
        private readonly string _discoveryFile;
        public AzureConnector(string connectionString, string rootDir, string discoveryFile, ICsvService csvService)
        {
            _csvService = csvService;
            _discoveryFile = discoveryFile;

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _baseUrl = $"{_storageAccount.BlobStorageUri.PrimaryUri}{rootDir}";
        }
        
        public async Task<AzureCache> DiscoveryMode()
        {
            var url = $"{_baseUrl}/{_discoveryFile}";
            var uri = new Uri(url);
            if (!await BlobForUrlExist(uri))
            {
                return null;
            }

            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            var rootMetadataRef = await cloudBlobClient.GetBlobReferenceFromServerAsync(uri);
            await rootMetadataRef.FetchAttributesAsync();
                
            var stream = new MemoryStream();
            await rootMetadataRef.DownloadToStreamAsync(stream);
            var devicesList = _csvService.ParseMetadataInfoFromStream(stream);

            var file = new AzureFile { Length = rootMetadataRef.Properties.Length, Name = rootMetadataRef.Name, Uri = rootMetadataRef.Uri };
                
            return new AzureCache { BaseUrl = _baseUrl, File = file, Devices = devicesList };
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

        public async Task<bool> BlobForUrlExist(Uri uri)
        {
            var cloudBlob = new CloudBlob(uri);
            return await cloudBlob.ExistsAsync();
        }
    }
}
