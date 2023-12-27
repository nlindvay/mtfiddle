using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mtfiddle.Application;

namespace mtfiddle;

public class ClaimCheckProvider : IClaimCheckProvider
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly ILogger<ClaimCheckProvider> _logger;
    private readonly ClaimCheckType TypeOptions;

    public ClaimCheckType ClaimCheckType => throw new System.NotImplementedException();

    public ClaimCheckProvider(BlobServiceClient blobServiceClient, ILogger<ClaimCheckProvider> logger, IOptions<ClaimCheckType> typeOptions)
    {
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("mtfiddle");
        _blobContainerClient.CreateIfNotExists();
        _logger = logger;
        TypeOptions = typeOptions.Value;
    }

    public async Task<Result<ClaimCheck?>> GetAsync(string uri)
    {
        BlobClient blobClient = _blobContainerClient.GetBlobClient(uri);
        var result = await blobClient.DownloadContentAsync();

        _logger.LogInformation($"ClaimCheckProvider: {result.GetRawResponse().Status} {result.GetRawResponse().ReasonPhrase}");

        if (result.GetRawResponse().Status != 200)
            return Result<ClaimCheck?>.Error(result.GetRawResponse().ReasonPhrase);
        return Result<ClaimCheck?>.Ok(new ClaimCheck(uri, result.Value.Content.ToString()));
    }

    public async Task<Result<ClaimCheck?>> CreateAsync(string content)
    {
        BlobClient client = _blobContainerClient.GetBlobClient(NewId.NextGuid().ToString());
        var result = await client.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(content)));

        _logger.LogInformation($"ClaimCheckProvider: {result.GetRawResponse().Status} {result.GetRawResponse().ReasonPhrase}");

        if (result.GetRawResponse().Status != 201)
            return Result<ClaimCheck?>.Error(result.GetRawResponse().ReasonPhrase);
        return Result<ClaimCheck?>.Ok(new ClaimCheck(client.Name, content));
    }
}