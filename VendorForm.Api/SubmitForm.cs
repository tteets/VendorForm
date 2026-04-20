using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using VendorForm.Api.Models;
using VendorForm.Api.Services;

namespace VendorForm.Api;

public class SubmitForm
{
    private readonly StorageContext _storage = null;
    public SubmitForm(StorageContext storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    [Function("CreatePendingSubmission")]
    public async Task<HttpResponseData> CreatePendingSubmission([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var vendorInfo = await req.ReadFromJsonAsync<VendorInformation>();

        if (vendorInfo is null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("VendorInformation payload was null or invalid JSON.");
            return bad;
        }

        var validationErrors = ValidationHelper.ValidateVendor(vendorInfo);
        if (validationErrors.Any())
        {

            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new
            {
                Message = "Validation failed.",
                Errors = validationErrors
            });
            return bad;
        }

        var submissionId = Guid.NewGuid().ToString("N");
        var uploadToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        var entity = new VendorSubmissionEntry
        {
            PartitionKey = "VendorSubmission",
            RowKey = submissionId,

            UploadToken = uploadToken,
            Status = "PendingFile",
            CreatedUtc = DateTimeOffset.UtcNow,

            VendorName = vendorInfo.VendorName,
            PayeeName = vendorInfo.PayeeName,
            Address = vendorInfo.Address,
            Address2 = vendorInfo.Address2,
            City = vendorInfo.City,
            State = vendorInfo.State,
            Zipcode = vendorInfo.Zipcode,
            Email = vendorInfo.Email,
            EntityType = vendorInfo.EntityType,
            ContactName = vendorInfo.ContactName,
            ContactPhone = vendorInfo.ContactPhone,
            Ext = vendorInfo.Ext,
            AccountType = vendorInfo.AccountType ?? string.Empty,
            BankName = vendorInfo.BankName ?? string.Empty,

            FederalTaxId = CryptoHelper.Encrypt(vendorInfo.FederalTaxId),
            SSN = CryptoHelper.Encrypt(vendorInfo.SSN),
            LSUId = CryptoHelper.Encrypt(vendorInfo.LSUId),
            RoutingNumber = CryptoHelper.Encrypt(vendorInfo.RoutingNumber),
            AccountNumber = CryptoHelper.Encrypt(vendorInfo.AccountNumber)
        };

        if (_storage is null || _storage.VendorTable is null)
            throw new InvalidOperationException("There is an issue with Azure Storage. Either there is no reference or the 'VendorSubmission' table was not created properly.");


        await _storage.VendorTable.AddEntityAsync<VendorSubmissionEntry>(entity);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            SubmissionId = submissionId,
            UploadToken = uploadToken
        });

        return response;
    }


    [Function("UploadVendorFile")]
    public async Task<IActionResult> UploadVendorFile([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        if(!req.Headers.TryGetValue("Authorization", out var authHeaders)) return new UnauthorizedResult();

        var token = authHeaders.ToString().Replace("UploadToken ", "");

        var form = await req.ReadFormAsync();

        if (!form.TryGetValue("SubmissionId", out var submissionIdValues)) return new BadRequestObjectResult("Missing SubmissionId");
        var submissionId = submissionIdValues.FirstOrDefault();
        if (string.IsNullOrEmpty(submissionId)) return new BadRequestObjectResult("Missing SubmissionId");

        if (form.Files.Count != 1) return new BadRequestObjectResult("Exactly one file must be uploaded.");
        var file = form.Files[0];

        var entityResponse = await _storage.VendorTable.GetEntityAsync<VendorSubmissionEntry>("VendorSubmission", submissionId);
        var entity = entityResponse.Value;
        if (entity.Status != "PendingFile" || entity.UploadToken != token) return new ForbidResult();

        var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension)) return new BadRequestObjectResult("Invalid file type.");

        int MaxFileSize = 10 * 1024 * 1024; //10MB
        if (file.Length > MaxFileSize) return new BadRequestObjectResult("File exceeds 10MB limit.");

        var blobName = $"{submissionId}/attachment{extension}";
        var blob = _storage.VendorBlobContainer.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, overwrite: true);

        entity.UploadToken = "";
        entity.Status = "UploadComplete";
        entity.UploadedUtc = DateTimeOffset.Now;
        await _storage.VendorTable.UpdateEntityAsync(entity, entity.ETag, Azure.Data.Tables.TableUpdateMode.Merge);

        return new OkResult();
    }


    [Function("RollbackSubmission")]
    public async Task<HttpResponseData> RollbackSubmission([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var requestSubmission = await req.ReadFromJsonAsync<SubmissionIdRequest>();

        if (requestSubmission is null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid payload.");
            return badRequest;
        }

        try
        {
            await _storage.VendorTable.DeleteEntityAsync(
                "VendorSubmission", requestSubmission.SubmissionId);
        }catch(RequestFailedException ex) when (ex.Status == 404)
        {
            //Already has been done
        }

        return req.CreateResponse(HttpStatusCode.OK);
    }
}

public record SubmissionIdRequest(string SubmissionId);