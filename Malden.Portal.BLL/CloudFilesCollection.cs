using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.BLL
{
    public class CloudFilesModel
    {
        public CloudFilesModel()
            :this(null, null)
        {
            Files = new List<CloudFile>();
            
        }

        public CloudFilesModel(IEnumerable<IListBlobItem> list, IBlobManagerLogic blobManagerLogic)
        {
            Files = new List<CloudFile>();
            var cdnEndpoint = blobManagerLogic.CDNEndPoint();
            try
            {
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        CloudFile info = CloudFile.CreateFromIListBlobItem(item, cdnEndpoint);
                        if (info != null)
                        {
                            Files.Add(info);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(Microsoft.WindowsAzure.Storage.StorageException))
                {
                    Files = new List<CloudFile>();
                }
            }
        }

        public static List<CloudFile> FilteredFiles(string releaseId, IReleaseLogic releaseLogic, IProductLogic productLogic, IBlobManagerLogic blobLogic)
        {
            var files = new List<CloudFile>();

            var release = releaseLogic.GetById(releaseId);

            if (release == null) return files;
            var containerName = productLogic.GetById(release.ProductId).ContainerName;

            var storageAccount = CloudStorageAccount.Parse(releaseLogic.ReleaseConnectionKey());
            var storageClient = storageAccount.CreateCloudBlobClient();

            var values = blobLogic.ListOfAvailableBlobs(releaseId);

            foreach (var fileType in values)
            {
                var imageType = (Malden.Portal.BLL.Release.ImageFileType)fileType;

                var fileTypeContainerName = containerName + "-" + imageType.ToString().ToLower();

                if (storageClient.GetContainerReference(fileTypeContainerName).Exists())
                {
                    var storageContainer = storageClient.GetContainerReference(fileTypeContainerName);

                    var blobs = new CloudFilesModel(storageContainer.ListBlobs(useFlatBlobListing: true), blobLogic);

                    Console.Write("Release:" + releaseId);

                    foreach (var blob in blobs.Files.Where(b => b.ReleaseId == releaseId))
                    {
                        files.Add(new CloudFile
                        {
                            FileName = blob.FileName,
                            Size = blob.Size,
                            URL = blob.URL,
                            ImageFileType = imageType,
                            ContainerName = blob.ContainerName
                        });
                    }
                }
            }
            return files;
        }

        public List<CloudFile> Files { get; set; }

        public static bool IsBlobContainerExists(CloudBlockBlob blob)
        {
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}