using System.Reflection;
using Azure.Storage.Blobs;

namespace VaderHinna.AzureDataSetup
{
    public class AzureSetup
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _device;

        private const string Date = "2019-01-10";
        private const string Csv = ".csv";

        public AzureSetup(string connectionString, string containerName, string device)
        {
            _connectionString = connectionString;
            _containerName = containerName;
            _device = device;
        }
        public void Setup()
        {
            var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);
            blobContainerClient.CreateIfNotExists();

            var assembly = Assembly.GetExecutingAssembly();
            var currentNamespace = assembly.GetName().Name;
            var sensors = GetTestSensors();
            foreach (var sensor in sensors)
            {
                var fullname = $"{currentNamespace}.Resources.{_device}.{sensor}.{Date}{Csv}";
                var stream = assembly.GetManifestResourceStream(fullname);
                var file = new BlobClient(_connectionString, _containerName, $"{_device}/{sensor}/{Date}{Csv}");
                if (!file.Exists())
                {
                    file.Upload(stream);
                }
            }
        }

        public string[] GetTestSensors()
        {
            return new[] { "humidity", "rainfall", "temperature" };
        }

        public void Clear()
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            blobServiceClient.DeleteBlobContainer(_containerName);
        }
    }
}
