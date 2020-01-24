# VaderHinna

Find out past or present values of your IoT device sensor with this API

## Prerequisites

- .Net Core 3.0 SDK
- [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) (for running tests and local development)

## Setup

1. Run project in Visual Studio as Admin
2. Right click main (VaderHinna) project and select "Manage User Secrets"
3. Paste following to opened secrets.json and fill data

```JSON
{
  "ConnectionString": "",
  "RootDirectory": "",
  "DiscoveryFile": ""
}
```

4. Save file
5. Build solution
6. API now can be started (Make sure **VaderHinna** is set as Start Up Project)

## Projects in solution

- **VaderHinna** main API project.

* **VaderHinna.AzureDataSetup**
  Class library that has options to set up data on local Azure Storage Emulator

* **VaderHinna.LocalSetup**
  If you want to setup Blob Container on your local PC, launch this project
* **VaderHinna.Test** Testpack to check functionality of Service

**Be sure to run Azure Storage Emulator first, otherwise tests will fail**

## Limitations

- Azure Storage Emulator doesn't support AppendBlobs, and this piece isn't covered by tests
