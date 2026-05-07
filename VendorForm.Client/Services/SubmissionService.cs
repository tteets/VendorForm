using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using VendorForm.Client.Models;

namespace VendorForm.Client.Services;

public class SubmissionService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    public SubmissionService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<SubmissionResult> CreatePendingSubmission(VendorInformation vendorInfo)
    {
        var url = "/api/createpending";

        var response = await _http.PostAsJsonAsync(url, vendorInfo);
        SubmissionResult? result;

        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<SubmissionResult>();
        }else
        {
            throw new Exception("There was an error getting information from the pending submission");
        }

        return new SubmissionResult
        {
            SubmissionId = result!.SubmissionId,
            UploadToken = result!.UploadToken
        };
    }

    public async Task RollbackSubmission(string submissionId)
    {
        //Remove the line in Azure Storage due to file upload error
        var url = "/api/rollback";
        var response = await _http.PostAsJsonAsync(url, new SubmissionId(submissionId));
    }
}
public record SubmissionId(string submissionId);
