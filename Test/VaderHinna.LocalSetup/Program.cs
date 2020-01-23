using System;
using VaderHinna.AzureDataSetup;

namespace VaderHinna.LocalSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            var setup = new AzureSetup();
            Console.WriteLine("This will set up Blob Container on your local Azure Storage Emulator");
            setup.Setup();
        }
    }
}
