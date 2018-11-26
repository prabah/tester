using Malden.Portal.Data;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.BLL
{

    public class BlobManagerLogic : IBlobManagerLogic
    {
        private readonly IReleaseLogic _releaseLogic;
        private readonly IProductLogic _productLogic;
        private readonly IBlobManagerRepository _blobManagerRepository;
        private readonly IBlobManagerContainerGetter _blobGetter;

        public BlobManagerLogic(IReleaseLogic releaseLogic, IProductLogic productLogic, IBlobManagerRepository blobManagerRepository, IBlobManagerContainerGetter blobGetter)
        {
            _releaseLogic = releaseLogic;
            _productLogic = productLogic;
            _blobManagerRepository = blobManagerRepository;
            _blobGetter = blobGetter;
        }

        public void RefreshBlobFileCount(string releaseId)
        {
            var release = _releaseLogic.GetById(releaseId);
            var containerName = _productLogic.GetById(release.ProductId).ContainerName;

            //TODO Remove
            var storageAccount = CloudStorageAccount.Parse(_releaseLogic.ReleaseConnectionKey());
            var storageClient = storageAccount.CreateCloudBlobClient();

            var fileTypesCount = Enum.GetNames(typeof(Malden.Portal.BLL.Release.ImageFileType)).Length;
            

            var values = from Malden.Portal.BLL.Release.ImageFileType e in
                             Enum.GetValues(typeof(Malden.Portal.BLL.Release.ImageFileType))
                         select new { Id = (int)e };


            foreach (var fileType in values)
            {
                var fileTypeContainerName = containerName + "-" + ((Malden.Portal.BLL.Release.ImageFileType)fileType.Id).ToString().ToLower();
                var storageContainer = storageClient.GetContainerReference(fileTypeContainerName);

                var blobs = new CloudFilesModel(storageContainer.ListBlobs(useFlatBlobListing: true), this);

                var blob = (from b in blobs.Files
                                 where (b.ReleaseId == releaseId)
                                 select b).FirstOrDefault();

                var fileName = blob != null ? blob.FileName : "";
                var fileSize = blob != null ? blob.Size : 0;


                var blobFileName = blob != null ? new Uri(blob.URL).PathAndQuery : "";

                var URL = blob != null ? blob.URL : "";

                _blobManagerRepository.AddBlobCount(fileType.Id, releaseId, blob != null ? 1 : 0, fileName, fileSize, blobFileName);

            }
        }

        public IList<BLL.BlobManager> CountOfBlobFiles(string releaseId)
        {
            return _blobManagerRepository.CountOfBlobFiles(releaseId).Select(c => new BlobManager
            {
                FileType = c.FileType,
                NoOfFiles = c.NoOfFiles,
                ReleaseId = c.ReleaseId,
                FileName = c.FileName,
                FileURL = c.FileURL,
                FileSize = c.FileSize
            }).ToList();
        }


        public void AddBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL)
        {
            _blobManagerRepository.AddBlobCount(fileTypeId, releaseId, numberOfFiles, fileName, fileSize, fileURL);
        }

        public void UpdateBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL)
        {
            _blobManagerRepository.UpdateBlobCount(fileTypeId, releaseId, numberOfFiles, fileName, fileSize, fileURL);
        }

        public IList<BLL.BlobManager> BlobFileCount(string releaseId)
        {
            return _blobManagerRepository.CountOfBlobFiles(releaseId).Select(c => new BLL.BlobManager
            {
                FileType = c.FileType,
                NoOfFiles = c.NoOfFiles,
                ReleaseId = c.ReleaseId,
                FileURL = c.FileURL,
                FileSize = c.FileSize,
                FileName = c.FileName
            }).ToList(); 
        }


        public Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer GetContainer(string containerName)
        {
            return _blobGetter.GetContainer(containerName);
        }


        public IList<int> ListOfAvailableBlobs(string releaseId)
        {
            return _blobManagerRepository.ListOfAvailableBlobs(releaseId);
        }

        public IList<CloudFile> CloudFiles(string releaseId)
        {
            var list = new List<CloudFile>();

            var blobs = CountOfBlobFiles(releaseId);
            var cdnEndpoint = _blobManagerRepository.CDNEndPoint();


            foreach (var blob in blobs)
            {
                if (blob.FileSize > 0)
                {
                    var cloudFile = new CloudFile
                    {
                        FileName = blob.FileName,
                        URL = blob.FileURL.IndexOf(cdnEndpoint) == 0 ? blob.FileURL :  cdnEndpoint + blob.FileURL,
                        Size = blob.FileSize,
                        ReleaseId = releaseId,
                        ImageFileType = (Release.ImageFileType)blob.FileType,
                        ContainerName = _productLogic.GetById(_releaseLogic.GetById(releaseId).ProductId).ContainerName + "-" + ((Release.ImageFileType)blob.FileType).ToString().ToLower()
                    };
                    list.Add(cloudFile);
                }
            }

            return list;
        }


        public string CDNEndPoint()
        {
            return _blobManagerRepository.CDNEndPoint();
        }
    }


}
