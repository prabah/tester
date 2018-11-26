using Malden.Portal.BLL;
using Ninject;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace Malden.Portal.Service.WebNew
{
    public class CustomValidator : UserNamePasswordValidator
    {
        private readonly IKernel _kernel;

        public CustomValidator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Validate(string userName)
        {
            var userLogic = _kernel.Get<IUserLogic>();

            if (!userLogic.IsValidUser(userName))
                throw new SecurityTokenException("Unknown Errrors occurred!");
        }

        public override void Validate(string userName, string passWord)
        {
            var userLogic = _kernel.Get<IUserLogic>();

            var user = new User(userName, passWord);

            if (!userLogic.IsValidAdminUser(user, passWord))
                throw new SecurityTokenException("Unknown Errrors occurred!");
        }
    }
}