using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendorForm.Api;


public class StorageContext
{
    public TableClient VendorTable { get; }
    public BlobContainerClient VendorBlobContainer;


    public StorageContext()
    {
        var connectionString =
            Environment.GetEnvironmentVariable("AzureConnectionString") 
            ?? throw new InvalidOperationException("AzureConnectionString is missing");
            

        VendorTable = new TableClient(connectionString, "VendorSubmissions");
        VendorTable.CreateIfNotExists();

        var blobService = new BlobServiceClient(connectionString);
        VendorBlobContainer = blobService.GetBlobContainerClient("vendor-submissions");
        VendorBlobContainer.CreateIfNotExists();
    }

}

