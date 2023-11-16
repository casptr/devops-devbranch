namespace Foodtruck.Shared.Supplements;

public abstract class SupplementResult
{
    public class Index
    {
        public IEnumerable<SupplementDto.Detail>? Supplements { get; set; }
        public int TotalAmount { get; set; }
    }

    public class Mutate
    {
        public int SupplementId { get; set; }
        public IList<Uri>? UploadSasUris { get; set; }
    }
}
