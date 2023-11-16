using Microsoft.AspNetCore.Components.Forms;

namespace Foodtruck.Client.BlobFiles;

public interface IStorageService
{
    Task UploadImageAsync(string sas, IBrowserFile file);
}