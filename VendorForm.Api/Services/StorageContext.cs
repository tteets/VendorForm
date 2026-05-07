using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace VendorForm.Api.Services;


public class StorageContext
{
    public TableClient VendorTable { get; }
    public BlobContainerClient VendorBlobContainer;


    public StorageContext()
    {
        var connectionString =
            Environment.GetEnvironmentVariable("AzureWebJobsStorage") 
            ?? throw new InvalidOperationException("AzureConnectionString is missing");
            

        VendorTable = new TableClient(connectionString, "VendorSubmissions");
        VendorTable.CreateIfNotExists();

        var blobService = new BlobServiceClient(connectionString);
        VendorBlobContainer = blobService.GetBlobContainerClient("vendor-submissions");
        VendorBlobContainer.CreateIfNotExists();
    }

}

