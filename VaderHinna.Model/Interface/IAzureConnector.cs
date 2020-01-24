using System;
using System.Threading.Tasks;

namespace VaderHinna.Model.Interface
{
    public interface IAzureConnector
    {
        Task<AzureCache> DiscoveryMode();
        Task<string> DownloadTextByAppendUri(Uri uri);
        Task<bool> BlobForUrlExist(Uri uri);
    }
}
