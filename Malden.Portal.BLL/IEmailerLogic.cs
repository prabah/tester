
namespace Malden.Portal.BLL
{
    public interface IEmailerLogic
    {
        void Add(User user, string uniqueId, Malden.Portal.BLL.EmailerLogic.EmailType emailType);
    }
}
