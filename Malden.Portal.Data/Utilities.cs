using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Malden.Portal.Data.TableStorage
{
    internal static class Utilities
    {
        internal static void Start(string connectionString)
        {
            //(CDLTLL) Configuration for Windows Azure settings
            CloudStorageAccount.SetConfigurationSettingPublisher(
                (configName, configSettingPublisher) => configSettingPublisher(connectionString));
        }

        public static void SetNotFoundException(TableServiceContext context)
        {
            context.IgnoreResourceNotFoundException = true;
        }
    }
}