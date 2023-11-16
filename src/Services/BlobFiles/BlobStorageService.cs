using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Domain.BlobFiles;
using Microsoft.Extensions.Configuration;

namespace Services.BlobFiles;

public class BlobStorageService : IStorageService
{
    public Uri BasePath => new("https://blanchestorage.blob.core.windows.net/images");

    private readonly string containerName = "images";
    private readonly BlobContainerClient containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        containerClient = new BlobServiceClient(configuration["Azure:BlobStorage"])
                                .GetBlobContainerClient(containerName);
    }

    public Uri GenerateImageUploadSas(Image image)
    {
        BlobClient blobClient = containerClient.GetBlobClient(image.Filename);

        var blobSasBuilder = new BlobSasBuilder
        {
            ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            BlobContainerName = containerName,
            BlobName = image.Filename,
        };

        blobSasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);
        var sas = blobClient.GenerateSasUri(blobSasBuilder);
        return sas;
    }

    public async Task<bool> DeleteBlobFile(Uri file)
    {
        if (!file.ToString().Contains(BasePath.ToString()))
            return false;

        BlobClient blobClient = containerClient.GetBlobClient(file.ToString().Split('/').Last());
        return await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
    }
}

