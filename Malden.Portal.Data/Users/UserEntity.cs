using Microsoft.WindowsAzure.StorageClient;

namespace Malden.Portal.Data.TableStorage.Users
{
    public class UserEntity : TableServiceEntity, IUser
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Company { get; set; }

        //public bool IsAdmin { get; set; }

        public int TypeOfUser { get; set; }

        public string Id { get; set; }

        public bool IsBlocked { get; set; }

        public int RegisteredProductsCount { get; set; }
    }
}