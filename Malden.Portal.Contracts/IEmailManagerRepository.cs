
namespace Malden.Portal.Data
{
    public interface IEmailManagerRepository
    {
        void Add(IEmailManager emailInstance);
        IEmailManager Get(string id, int emailType);
        bool IsEmailExpired(string id, int emailType);
        void Activate(string id, int emailType);
        bool IsAwaitingActivation(string userId);
        string GetEmailByKey(string key);
    }
}
