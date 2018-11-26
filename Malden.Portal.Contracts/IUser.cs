namespace Malden.Portal.Data
{
    public interface IUser
    {
        string Email { get; set; }

        string Name { get; set; }

        string Password { get; set; }

        string Company { get; set; }

        string Id { get; set; }

        int TypeOfUser { get; set; }

        bool IsBlocked { get; set; }

        int RegisteredProductsCount { get; set; }

        //int UserType { get; set; }
    }
}