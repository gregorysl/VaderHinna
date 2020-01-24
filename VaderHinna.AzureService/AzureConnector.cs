using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VaderHinna.Model;
using VaderHinna.Model.Interface;

namespace VaderHinna.AzureService
{
    public class AzureConnector : IAzureConnector
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly ICsvService _csvService;
        public readonly string BaseUrl;
        private readonly string _discoveryFile;
        public AzureConnector(string connectionString, string rootDir, string discoveryFile, ICsvService csvService)
        {
            _csvService = csvService;
            _discoveryFile = discoveryFile;

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
            BaseUrl = $"{storageAccount.BlobStorageUri.PrimaryUri.ToString().TrimEnd('/')}/{rootDir}/";
        }
        
        public async Task<AzureCache> DiscoveryMode()
        {
            var url = $"{BaseUrl}{_discoveryFile}";
            var uri = new Uri(url);
            if (!await BlobForUrlExist(uri))
            {
                return null;
            }

            var rootMetadataRef = await _cloudBlobClient.GetBlobReferenceFromServerAsync(uri);
                
            var stream = new MemoryStream();
            await rootMetadataRef.DownloadToStreamAsync(stream);
            var devicesList = _csvService.ParseMetadataInfoFromStream(stream);
    
            return new AzureCache { BaseUrl = BaseUrl, Devices = devicesList };
        }

        public async Task<string> DownloadTextByAppendUri(Uri uri)
        {
            var blob = new CloudAppendBlob(uri,_cloudBlobClient);
            var content = await blob.DownloadTextAsync();
            return content;
        }

        public async Task<bool> BlobForUrlExist(Uri uri)
        {
            var cloudBlob = new CloudBlob(uri, _cloudBlobClient);
            return await cloudBlob.ExistsAsync();
        }
    }
}
