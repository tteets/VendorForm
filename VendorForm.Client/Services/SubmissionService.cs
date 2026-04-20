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

        var result = await response.Content.ReadFromJsonAsync<SubmissionResult>();

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

    public async Task MarkCompleted(string submissionId)
    {
        //Mark the submission row complete. This means the file has been successfully uploaded to Blob Storage
        var url = "/api/complete";
        var response = await _http.PostAsJsonAsync(url, new SubmissionId(submissionId));
    }
}
public record SubmissionId(string submissionId);
