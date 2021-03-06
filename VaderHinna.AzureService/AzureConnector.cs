﻿using System;
using System.Collections.Generic;
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
        private readonly string _baseUrl;
        private readonly string _discoveryFile;
        public AzureConnector(string connectionString, string rootDir, string discoveryFile, ICsvService csvService)
        {
            _csvService = csvService;
            _discoveryFile = discoveryFile;

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
            _baseUrl = $"{storageAccount.BlobStorageUri.PrimaryUri.ToString().TrimEnd('/')}/{rootDir}/";
        }


        public string CreateUrl(string end) => $"{_baseUrl.TrimEnd('/')}/{end}";

        public async Task<List<AzureDevice>> DeviceDiscovery()
        {
            var url = $"{_baseUrl}{_discoveryFile}";
            var uri = new Uri(url);
            if (!await BlobForUrlExist(uri))
            {
                return null;
            }

            var rootMetadataRef = await _cloudBlobClient.GetBlobReferenceFromServerAsync(uri);

            await using var stream = new MemoryStream();
            await rootMetadataRef.DownloadToStreamAsync(stream);
            var devicesList = _csvService.ParseMetadataInfoFromStream(stream);
    
            return devicesList;
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
            if (cloudBlob.Container.Uri.ToString().TrimEnd('/') == uri.ToString().TrimEnd('/')) return false;
            return await cloudBlob.ExistsAsync();
        }
    }
}
