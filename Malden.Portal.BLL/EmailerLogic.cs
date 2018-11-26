using Malden.Portal.Data;

namespace Malden.Portal.BLL
{
    public class EmailerLogic : IEmailerLogic
    {
        private readonly IEmailManager _emailManager;
        private readonly IEmailManagerRepository _emailRepository;

        public enum EmailType
        {
            Welcome = 1,
            ResetPassword = 2
        }


        public EmailerLogic(IEmailManager emailManager, IEmailManagerRepository emailRepository)
        {
            _emailManager = emailManager;
            _emailRepository = emailRepository;
        }

        public void Add(User user, string uniqueId, EmailType emailType)
        {
            _emailManager.Id = uniqueId;
            _emailManager.UserId = user.Id;
            _emailManager.EmailType = (int)emailType;
            _emailRepository.Add(_emailManager);
        }
    }
}