
namespace Malden.Portal.Data
{
    public interface IDistributor
    {
        string Id { get; set; }
        string Email { get; set; }
        string Token { get; set; }
        bool IsRegistered { get; set; }
        bool IsActivated { get; set; }
    }
}
