using System;
using System.Threading.Tasks;

namespace VaderHinna.Model.Interface
{
    public interface IAzureConnector
    {
        Task<AzureCache> DeviceDiscovery();
        Task<string> DownloadTextByAppendUri(Uri uri);
        Task<bool> BlobForUrlExist(Uri uri);
    }
}
