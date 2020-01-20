using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaderHinna.Model.Interface
{
    public interface IAzureConnector
    {
        Task<AzureCache> DiscoveryMode();
        Task<string> DownloadTextByBlobUri(Uri uri);
        Task<string> DownloadTextByAppendUri(Uri uri);
        Task<List<SensorData>> DownloadDeviceDataForSensor(Uri uri);
        Task<bool> CheckBlobUrlExist(Uri uri);
    }
}
