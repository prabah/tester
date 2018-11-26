namespace Malden.Portal.Data
{
    public interface IUserPurchase
    {
        string Id { get; set; }

        string UserId { get; set; }

        string RegistrationCode { get; set; }

        string ProductId { get; set; }

        int SerialNumber { get; set; }

        bool IsMaintenanceAvailable { get; set; }
    }
}