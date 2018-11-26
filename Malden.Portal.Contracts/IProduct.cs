namespace Malden.Portal.Data
{
    public interface IProduct
    {
        string Id { get; set; }

        string Name { get; set; }

        string ContainerName { get; set; }

        bool IsMaintained { get; set; }

        int FulfilmentId { get; set; }
    }
}