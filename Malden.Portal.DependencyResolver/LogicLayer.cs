using Malden.Portal.BLL;
using Ninject.Modules;

namespace Malden.Portal.DependencyResolver
{
    public class LogicLayer : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserLogic>().To<UserLogic>();
            Bind<IProductLogic>().To<ProductLogic>();
            Bind<IBlobManagerLogic>().To<BlobManagerLogic>();
            Bind<IReleaseLogic>().To<ReleaseLogic>();
            Bind<IUserPurchaseLogic>().To<UserPurchaseLogic>();
            Bind<IProductCatalogueLogic>().To<ProductCatalogueLogic>();
            Bind<IMaintenanceContractLogic>().To<MaintenanceContractLogic>();
            Bind<IHistoryLogic>().To<HistoryLogic>();
            Bind<IEmailerLogic>().To<EmailerLogic>();
            Bind<IDistributorLogic>().To<DistributorLogic>();
        }
    }
}
