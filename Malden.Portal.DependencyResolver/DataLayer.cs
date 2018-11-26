using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage;
using Malden.Portal.Data.TableStorage.Distributors;
using Malden.Portal.Data.TableStorage.Emails;
using Malden.Portal.Data.TableStorage.History;
using Malden.Portal.Data.TableStorage.MaintenanceContracts;
using Malden.Portal.Data.TableStorage.Products;
using Malden.Portal.Data.TableStorage.ProductSerialNumbers;
using Malden.Portal.Data.TableStorage.Releases;
using Malden.Portal.Data.TableStorage.UserPurchases;
using Malden.Portal.Data.TableStorage.Users;
using Ninject.Modules;

namespace Malden.Portal.DependencyResolver
{
    public class DataLayer : NinjectModule
    {
        public override void Load()
        {

            Tables.CreateTablesIfNotExist();

#if (!DEBUG)

            Bind<IUserRepository>().To<UserRepository>();
            Bind<IUser>().To<UserEntity>();

            //Product Types
            Bind<IProduct>().To<ProductEntity>();
            Bind<IProductRepository>().To<ProductRepository>();

            //Products
            Bind<IFileManager>().To<FileManager>();

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

            //History
            Bind<IHistory>().To<HistoryEntity>();
            Bind<IHistoryRepository>().To<HistoryRepository>();

            //BlobManagerRepository
            Bind<IBlobManager>().To<BlobManagerEntity>();
            Bind<IBlobManagerRepository>().To<BlobManagerRepository>();

            //EmailManagerRepository
            Bind<IEmailManager>().To<EmailManagerEntity>();
            Bind<IEmailManagerRepository>().To<EmailManagerRepository>();

            //BlobGetterRepository
            Bind<IBlobManagerContainerGetter>().To<BlobManagerContainerGetter>();
#else
            {
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
                Bind<IHistoryRepository>().To<HistoryRepository>();

                //BlobManagerRepository
                Bind<IBlobManager>().To<BlobManagerEntity>();
                Bind<IBlobManagerRepository>().To<BlobManagerRepository>();

                //EmailManagerRepository
                Bind<IEmailManager>().To<EmailManagerEntity>();
                Bind<IEmailManagerRepository>().To<EmailManagerRepository>();

                //BlobGetterRepository
                Bind<IBlobManagerContainerGetter>().To<BlobManagerContainerGetter>();

                //DistributorRepository
                Bind<IDistributor>().To<DistributorEntity>();
                Bind<IDistributorRepository>().To<DistributorRepository>();
            }
#endif

        }
    }
}