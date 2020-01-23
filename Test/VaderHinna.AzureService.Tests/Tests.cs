using NUnit.Framework;
using VaderHinna.AzureDataSetup;

namespace VaderHinna.AzureService.Tests
{
    public class Tests
    {
        private readonly AzureSetup _azureSetup;
        public Tests()
        {
            _azureSetup = new AzureSetup();
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