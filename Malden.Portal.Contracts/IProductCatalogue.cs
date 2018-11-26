namespace Malden.Portal.Data
{
    public interface IProductCatalogue
    {
        string Id { get; set; }

        string ProductId { get; set; }

        string CurrentReleaseId { get; set; }

        int SerialNumber { get; set; }
    }
}