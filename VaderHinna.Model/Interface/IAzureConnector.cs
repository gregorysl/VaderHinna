using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaderHinna.Model.Interface
{
    public interface IAzureConnector
    {
        string CreateUrl(string end);
        Task<List<AzureDevice>> DeviceDiscovery();
        Task<string> DownloadTextByAppendUri(Uri uri);
        Task<bool> BlobForUrlExist(Uri uri);
    }
}
