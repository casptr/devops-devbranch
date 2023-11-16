using Domain.BlobFiles;

namespace Services.BlobFiles;

public interface IStorageService
{
    Uri BasePath { get; }
    Uri GenerateImageUploadSas(Image image);
    Task<bool> DeleteBlobFile(Uri file);
}