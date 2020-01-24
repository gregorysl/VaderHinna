using System;
using System.Threading.Tasks;
using NUnit.Framework;
using VaderHinna.AzureDataSetup;

namespace VaderHinna.AzureService.Tests
{
    public class Tests
    {
        private readonly AzureSetup _azureSetup;
        private AzureConnector _azureConnector;
        private readonly CsvService _csvService;
        public Tests()
        {
            _azureSetup = new AzureSetup();
            _csvService = new CsvService();
        }
        [SetUp]
        public void Setup()
        {
            _azureSetup.Setup();
            _azureConnector = new AzureConnector(_azureSetup.ConnectionString, _azureSetup.ContainerName, null,
                _csvService);
        }

        [Test]
        public async Task BlobForUrlExist_DoesntExist()
        {
            var uri = new Uri($"{_azureConnector.BaseUrl}randomBlob");
            var check = await _azureConnector.BlobForUrlExist(uri);
            Assert.AreEqual(false, check);
        }

        [Test]
        public async Task BlobForUrlExist_Exist()
        {
            var uri = new Uri($"{_azureConnector.BaseUrl}dockan/humidity/2019-01-10.csv");
            var check = await _azureConnector.BlobForUrlExist(uri);
            Assert.AreEqual(true, check);
        }

        [Test, Ignore("for now Azure Storage Emulator doesn't support Append Blobs. Unignore when testing using other connectionstring")]
        public async Task DownloadTextByAppendUri_Exist()
        {
            var uri = new Uri($"{_azureConnector.BaseUrl}dockan/SmallSample.txt");
            var check = await _azureConnector.DownloadTextByAppendUri(uri);
            Assert.AreEqual(true, check);
        }

        [Test]
        public async Task DeviceDiscovery_DoesntExist()
        {
            var devices = await _azureConnector.DeviceDiscovery();
            Assert.AreEqual(null, devices);
        }

        [TearDown]
        public void TearDown()
        {
            _azureSetup.Clear();
        }
    }
}