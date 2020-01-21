using System.Collections.Generic;

namespace VaderHinna.Model
{
    public class AzureCache
    {
        public string BaseUrl { get; set; }
        public List<AzureDevice> Devices { get; set; }
    }
}