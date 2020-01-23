using NUnit.Framework;
using VaderHinna.AzureDataSetup;

namespace VaderHinna.AzureService.Tests
{
    public class Tests
    {
        private const string ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        private string ContainerName = "iotbackend";
        private string Device = "dockan";
        private const string Date = "2019-01-10";
        private const string Csv = ".csv";
        private readonly AzureSetup _azureSetup;
        public Tests()
        {
            _azureSetup = new AzureSetup(ConnectionString, ContainerName,Device);
        }
        [SetUp]
        public void Setup()
        {
            _azureSetup.Setup();
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(1,1);
        }
        
        [TearDown]
        public void TearDown()
        {
            _azureSetup.Clear();
        }
    }
}