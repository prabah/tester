using Malden.Portal.Data;

namespace Malden.Portal.Tests.UserPurchases
{
    public class TestUserPurchase : IUserPurchase
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string RegistrationCode { get; set; }

        public string ProductId { get; set; }

        public string CurrentReleaseId { get; set; }

        public int SerialNumber { get; set; }

        public bool IsMaintenanceAvailable { get; set; }

    }
}