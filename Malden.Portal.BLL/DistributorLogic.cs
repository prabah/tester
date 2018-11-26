using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Malden.Portal.BLL
{
    public class DistributorLogic : IDistributorLogic
    {
        private readonly IDistributorRepository _distributorRepository;
        private readonly IDistributor _distributor;
        private readonly IEmailManagerRepository _emailRepository;
        private readonly IUserLogic _userLogic;

        public DistributorLogic(IDistributorRepository distributorRepository, IDistributor distributor, IEmailManagerRepository emailRepository, IUserLogic userLogic)
        {
            _distributorRepository = distributorRepository;
            _distributor = distributor;
            _emailRepository = emailRepository;
            _userLogic = userLogic;
        }

        public string Add(Distributor distributor)
        {
            if (_distributorRepository.GetByEmail(distributor.Email) != null) { throw new DuplicateEntryException("Email address already exists"); }

            var user = _userLogic.GetByEmail(distributor.Email);
            var isRegisteredAsNonDistributor = false;

            if(user != null)
            {
                isRegisteredAsNonDistributor = user.TypeOfUser != Malden.Portal.BLL.User.UserType.Distributor ? true: false;
            }

            if (isRegisteredAsNonDistributor) { throw new DuplicateEntryException(String.Format("Email registered as {0}", user.TypeOfUser == Malden.Portal.BLL.User.UserType.Customer ? "a Customer": "an Administrator") ); } 

            _distributor.Id = Guid.NewGuid().ToString();
            _distributor.Token = Guid.NewGuid().ToString().Substring(0, 8);
            _distributor.Email = distributor.Email.ToLower();
            _distributor.IsActivated = false;
            _distributor.IsRegistered = false;
            
            _distributorRepository.Add(_distributor);

            return _distributor.Id;
        }

        public void Update(Distributor distributor)
        {
            _distributor.Id = distributor.Id;
            _distributor.Email = distributor.Email;
            _distributor.Token = distributor.Token;
            _distributor.IsActivated = distributor.IsActivated;
            _distributor.IsRegistered = distributor.IsRegistered;
            _distributorRepository.Update(_distributor);
        }

        public Distributor Get(string id)
        {
            var distribitorEntity = _distributorRepository.Get(id);

            if (distribitorEntity == null) throw new NotFoundException("Distributor details not found.");

            return ProcessDistributor(distribitorEntity);
        }

        public Distributor GetByEmail(string email)
        {
            var distribitorEntity = _distributorRepository.GetByEmail(email.ToLower());

            if (distribitorEntity == null) return null;
                
            return ProcessDistributor(distribitorEntity);
        }

        public IList<Distributor> List()
        {
            return _distributorRepository.List().Select(d =>  ProcessDistributor(d)).ToList();
        }

        private Distributor ProcessDistributor(IDistributor distributor)
        { 
            return new Distributor
            {
                Id = distributor.Id,
                Email = distributor.Email,
                Token = distributor.Token,
                IsRegistered = distributor.IsRegistered,
                IsActivated = distributor.IsActivated
            };
        }

        public void Delete(Distributor distributor)
        {
            _distributorRepository.Delete((IDistributor)distributor);
        }

        public void ActivateDistributor(string key, int emailType, string email, string token)
        {
            var distributor = _distributorRepository.GetByEmail(email);

            if (token != distributor.Token) { throw new InvalidTokenException("Invlaid token."); }
            _userLogic.ActivateUser(key, emailType, User.UserType.Distributor);

            distributor.IsRegistered = true;
            distributor.IsActivated = true;
            distributor.Token = "";
            _distributorRepository.Update(distributor);

        }

        public bool IsDistributorAcccount(string key)
        {
            var userId = _emailRepository.GetEmailByKey(key);

            var user = _userLogic.Get(userId);

            return _distributorRepository.GetByEmail(user.Email) != null;
        }

        public string EmailByDistributorToken(string token)
        {
            return _distributorRepository.GetByToken(token).Email;
        }
    }
}
