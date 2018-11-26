using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage.Distributors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;
using System;
using System.Linq;

namespace Malden.Portal.Tests.Distributors
{
    [TestClass]
    public class Distributor_Repository_Should
    {
        private readonly StandardKernel _kernel = new StandardKernel();
        private IDistributorRepository _distributorRepository;


        [TestInitialize]
        public void Setup()
        {
            _distributorRepository = new DistributorRepository(null);

            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);
        }

        [TestMethod]
        public void add()
        {
            var distributor = new DistributorEntity { Id = Guid.NewGuid().ToString(), Email = "cravichan@gmail.com", Token = Guid.NewGuid().ToString().Substring(1, 6) };
            _distributorRepository.Add(distributor);
            var results = _distributorRepository.Get(distributor.Id);
            Assert.AreEqual(distributor.Email, results.Email);
            Assert.AreEqual(distributor.Id, results.Id);
            Assert.AreEqual(distributor.Token, results.Token);
        }

        [TestMethod]
        public void get_with_email_or_id()
        {
            var distributor = new DistributorEntity { Id = "e0f82bef-8336-4d46-b611-371fc8237f6d", Email = "cravichan@gmail.com", Token = Guid.NewGuid().ToString().Substring(1, 6) };
            var distByEmail = _distributorRepository.GetByEmail(distributor.Email);
            var distById = _distributorRepository.Get(distributor.Id);

            Assert.AreSame(distByEmail, distById);
        }

        [TestMethod]
        public void get_all()
        {
            var list = _distributorRepository.List();
            Assert.AreEqual(4 , list.Count());
        }


        [TestMethod]
        public void delete()
        {
            var distributor = new DistributorEntity { Id = "e0f82bef-8336-4d46-b611-371fc8237f6d", Email = "cravichan@gmail.com", Token = Guid.NewGuid().ToString().Substring(1, 6) };
            _distributorRepository.Delete(distributor);
            Assert.IsNull(_distributorRepository.Get(distributor.Id));
        }


        //[TestCleanup]
        //public void clean_the_data()
        //{
        //    var all = _distributorRepository.List();
        //    foreach (var d in all)
        //    {
        //        _distributorRepository.Delete(d);
        //    }
        //}

    }
}
