using System.Reflection;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace VaderHinna.AzureDataSetup
{
    public class AzureSetup
    {
        public string ConnectionString = "AccountName=devstoreaccount1;" +
                                         "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
                                         "DefaultEndpointsProtocol=http;" +
                                         "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;" +
                                         "QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;" +
                                         "TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        public string ContainerName = "iotbackend";
        public string Device = "dockan";

        private const string Date = "2019-01-10";
        private const string Csv = ".csv";

        public void Setup()
        {
            var blobContainerClient = new BlobContainerClient(ConnectionString, ContainerName);
            blobContainerClient.CreateIfNotExists();

            var assembly = Assembly.GetExecutingAssembly();
            var currentNamespace = assembly.GetName().Name;
            var sensors = GetTestSensors();
            foreach (var sensor in sensors)
            {
                var fullname = $"{currentNamespace}.Resources.{Device}.{sensor}.{Date}{Csv}";
                var stream = assembly.GetManifestResourceStream(fullname);
                var file = new BlobClient(ConnectionString, ContainerName, $"{Device}/{sensor}/{Date}{Csv}");
                if (!file.Exists())
                {
                    file.Upload(stream);
                }
            }
            //TODO: for now Azure Storage Emulator doesn't support Append Blobs. Uncomment when testing using other connectionstring
            //var smallFileStream = assembly.GetManifestResourceStream($"{currentNamespace}.Resources.{Device}.SmallSample.txt");
            //var smallFile = new AppendBlobClient(ConnectionString, ContainerName, $"{Device}/SmallSample.txt");
            //if (!smallFile.Exists())
            //{
            //    smallFile.AppendBlock(smallFileStream);
            //}
        }

        public string[] GetTestSensors()
        {
            return new[] { "humidity", "rainfall", "temperature" };
        }

        public void Clear()
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            blobServiceClient.DeleteBlobContainer(ContainerName);
        }
    }
}
