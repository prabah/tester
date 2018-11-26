using Microsoft.WindowsAzure.StorageClient;

namespace Malden.Portal.Data.TableStorage.Distributors
{
    public class DistributorEntity : TableServiceEntity, IDistributor
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public bool IsRegistered { get; set; }

        public bool IsActivated { get; set; }

    }
}
