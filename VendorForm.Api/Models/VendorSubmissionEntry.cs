using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendorForm.Api.Models;

public class VendorSubmissionEntry : ITableEntity
{
    public string PartitionKey { get; set; } = "VendorSubmission";
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Status { get; set; } = "PendingFile";
    public string UploadToken { get; set; } = default!;
    public DateTimeOffset CreatedUtc { get; set; }

    public string VendorName { get; set; } = default!;
    public string PayeeName { get; set; }  = default!;
    public string Address { get; set; }  = default!;
    public string Address2 { get; set; } = default!;
    public string City { get; set; } = default!;
    public string? State { get; set; } = default!;
    public string Zipcode { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public string ContactName { get; set; } = default!;
    public string ContactPhone { get; set; } = default!;
    public string Ext { get; set; } = default!;

    //Encrypted
    public string FederalTaxId { get; set; } = default!;
    public string SSN { get; set; } = default!;
    public string LSUId { get; set; } = default!;
    public string AccountType { get; set; } = default!;
    public string BankName { get; set; } = default!;
    public string RoutingNumber { get; set; } = default!;
    public string AccountNumber { get; set; } = default!;
}
