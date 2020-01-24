using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using VaderHinna.AzureDataSetup;
using VaderHinna.Model;

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
            var uri = new Uri(_azureConnector.CreateUrl("randomBlob"));
            var check = await _azureConnector.BlobForUrlExist(uri);
            Assert.AreEqual(false, check);
        }

        [Test]
        public async Task BlobForUrlExist_Exist()
        {
            var uri = new Uri(_azureConnector.CreateUrl("dockan/humidity/2019-01-10.csv"));
            var check = await _azureConnector.BlobForUrlExist(uri);
            Assert.AreEqual(true, check);
        }

        [Test, Ignore("for now Azure Storage Emulator doesn't support Append Blobs. Unignore when testing using other connectionstring")]
        public async Task DownloadTextByAppendUri_Exist()
        {
            var uri = new Uri(_azureConnector.CreateUrl("dockan/SmallSample.txt"));
            var check = await _azureConnector.DownloadTextByAppendUri(uri);
            Assert.AreEqual(true, check);
        }

        [Test]
        public async Task DeviceDiscovery_DoesntExist()
        {
            var devices = await _azureConnector.DeviceDiscovery();
            Assert.AreEqual(null, devices);
        }

        [Test]
        public async Task DeviceDiscovery_Exist()
        {
            _azureConnector = new AzureConnector(_azureSetup.ConnectionString, _azureSetup.ContainerName, "metadata.csv",
                _csvService);
            var result = await _azureConnector.DeviceDiscovery();
            var sensors = _azureSetup.GetTestSensors().ToList().OrderBy(x => x);
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count,"Device count");
            Assert.AreEqual("dockan", result.First().Id,"Device Id");
            Assert.AreEqual(3, result.First().Sensors.Count,"Device Sensors count");
            Assert.AreEqual("dockan", result.First().Id,"Device Id");
            Assert.AreEqual(sensors, result.First().Sensors.OrderBy(x=>x),"List of Sensors");
        }

        [TearDown]
        public void TearDown()
        {
            _azureSetup.Clear();
        }
    }
}