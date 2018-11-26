using Malden.Portal.BLL;
using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage.History;
using Malden.Portal.Data.TableStorage.MaintenanceContracts;
using Malden.Portal.Data.TableStorage.Products;
using Malden.Portal.Data.TableStorage.ProductSerialNumbers;
using Malden.Portal.Data.TableStorage.Releases;
using Malden.Portal.Data.TableStorage.UserPurchases;
using Malden.Portal.Data.TableStorage.Users;
using Ninject.Modules;

namespace Malden.Portal.Tests
{
    public class Bootstrapper : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserLogic>().To<UserLogic>();
            Bind<IProductLogic>().To<ProductLogic>();
            Bind<IReleaseLogic>().To<ReleaseLogic>();
            Bind<IUserPurchaseLogic>().To<UserPurchaseLogic>();
            Bind<IProductCatalogueLogic>().To<ProductCatalogueLogic>();
            Bind<IMaintenanceContractLogic>().To<MaintenanceContractLogic>();
            Bind<IHistoryLogic>().To<HistoryLogic>();

            //Users
            Bind<IUserRepository>().To<UserRepository>();
            Bind<IUser>().To<UserEntity>();

            //Product Types
            Bind<IProduct>().To<ProductEntity>();
            Bind<IProductRepository>().To<ProductRepository>();

            //UserPurchases
            Bind<IUserPurchase>().To<UserPurchaseEntity>();
            Bind<IUserPurchaseRepository>().To<UserPurchaseRepository>();

            //Releases
            Bind<IRelease>().To<ReleaseEntity>();
            Bind<IReleaseRepository>().To<ReleaseRepository>();

            //Product Serial Numbers
            Bind<IProductCatalogue>().To<ProductSerialNumberEntity>();
            Bind<IProductCatalogueRepository>().To<ProductSerialNumberRepository>();


            //Maintenance Contract
            Bind<IMaintenanceContract>().To<MaintenanceContractEntity>();
            Bind<IMaintenanceContractRepository>().To<MaintenanceContractRepository>();

            //BLOB Managers
            Bind<IFileManager>().To<FileManager>();

            //History
            Bind<IHistory>().To<HistoryEntity>();
        }
    }
}