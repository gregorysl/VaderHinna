using System;
using System.Threading.Tasks;
using VaderHinna.Model;

namespace VaderHinna.AzureService
{
    public interface IAzureConnector
    {
        Task<AzureCache> DiscoveryMode();
        Task<string> DownloadTextByUri(Uri uri);
    }
}
