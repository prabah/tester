using Malden.Portal.BLL;
using Malden.Portal.BLL.Utilities;
using Malden.Portal.GUI.Azure.Webrole.Models;
using Malden.Portal.GUI.Azure.Webrole.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Malden.Portal.GUI.Azure.Webrole.Controllers
{
    [CustomAttributes.AdminOnly]
    public class ReleaseController : AuthoriseController
    {
        private readonly IProductLogic _productLogic;
        private readonly IReleaseLogic _releaseLogic;
        private readonly IBlobManagerLogic _blobLogic;
        private readonly IProductCatalogueLogic _productCatalogueLogic;

        public ReleaseController()
        {
        }

        public ReleaseController(IReleaseLogic releaseLogic, IProductLogic productLogic, IBlobManagerLogic blobLogic, IProductCatalogueLogic productCatalogueLogic)
        {
            _releaseLogic = releaseLogic;
            _productLogic = productLogic;
            _blobLogic = blobLogic;
            _productCatalogueLogic = productCatalogueLogic;
        }

        public ActionResult Create()
        {
            var viewModel = new ReleaseViewModel { Products = _productLogic.List() };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(ReleaseViewModel model, string releaseDate)
        {
            var viewModel = new ReleaseViewModel { Products = _productLogic.List() };
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            DateTime dateOfRelease;
            if (!DateTime.TryParse(releaseDate, out dateOfRelease))
            {
                ModelState.AddModelError("Release Date", "Invalid release date");
                return View(viewModel);
            }

            try
            {
                var newRelease = new Release(model.VersionString)
                {
                    DateOfRelease = dateOfRelease,
                    ProductId = model.ProductId
                };

                _releaseLogic.Add(newRelease);

                return RedirectToAction("Index");
            }
            catch (DuplicateEntryException exception)
            {
                this.Flash(exception.Message, FlashEnum.Error);

                return View(viewModel);
            }

            catch (Exception exception)
            {
                 ErrorLogger.Log(exception);
                this.Flash("Errors occurred during the process!", FlashEnum.Error);

                return View(viewModel);
            }
        }



        [HttpPost]
        public JsonResult Delete(string id)
        {
            if (_blobLogic.CloudFiles(id).Count > 0)
            {
                var errorMessage = "Please delete associated blobs before deleting the entry!";
                //this.Flash(errorMessage, FlashEnum.Error);
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
            }

            if (_productCatalogueLogic.IsReleaseHasProducts(id))
            {
                var errorMessage = "Please delete associated user purchases before deleting the entry!";
                //this.Flash(errorMessage, FlashEnum.Error);
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
            }

            try
            {
                _releaseLogic.Delete(id);

                return Json("Deleted", JsonRequestBehavior.AllowGet);
            }
            catch (ReferenceException exception)
            {
                this.Flash(exception.Message, FlashEnum.Error);
                return Json(exception.Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorLogger.Log(exception);
                this.Flash("Errors occurred during the process!", FlashEnum.Error);
                return Json(exception.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteImage(string id)
        {
            var splitterLocation = id.IndexOf("~");
            var containerName = id.Substring(0, splitterLocation);
            var fileName = id.Substring(splitterLocation + 1, (id.Length - (splitterLocation + 1)));
            var storageAccount = CloudStorageAccount.Parse(_releaseLogic.ReleaseConnectionKey());
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(containerName);
            var blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.FetchAttributes();

            blockBlob.Delete();
            _blobLogic.RefreshBlobFileCount(blockBlob.Metadata["ReleaseId"].ToString());
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string id)
        {
            var release = _releaseLogic.GetById(id);

            return View(new ReleaseViewModel
            {
                ProductId = release.ProductId,
                ProductName = _productLogic.GetById(release.ProductId).Name,
                VersionString = release.Version.ToString(),
                IsHidden = release.IsHidden,
                Release = release
            });
        }

        public ActionResult Edit(string id)
        {
            var release = _releaseLogic.GetById(id);
            var viewModel = new ReleaseViewModel
                                {
                                    ProductId = release.ProductId,
                                    ProductName = _productLogic.GetById(release.ProductId).Name,
                                    VersionString = release.Version.ToString(),
                                    Release = release,
                                    IsHidden = release.IsHidden,
                                    Products = _productLogic.List()
                                };

            return View(viewModel);
        }

        [HttpPost]
        [CustomAttributes.ValidateOnlyIncomingValues]
        public ActionResult Edit([Bind(Exclude = "Release.ImageFilePath")] ReleaseViewModel model, DateTime releaseDate, bool IsHidden)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var release = new Release
                    {
                        Id = model.Id,
                        DateOfRelease = releaseDate,
                        Version = new Version(model.VersionString.ToString()),
                        IsHidden = IsHidden,
                        ProductId = model.ProductId
                    };

                    _releaseLogic.Update(release);

                    this.Flash("Record updated!");

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }

            catch (Exception exception)
            {
                var viewModel = new ReleaseViewModel { Products = _productLogic.List() };
                var exType = exception.GetType();
                ErrorLogger.Log(exception);
                this.Flash("Errors occurred during the process!", FlashEnum.Error);
                return View(viewModel);
            }
        }

        public JsonResult FileStatus(string fileName, string releaseId, int fileType)
        {
            var release = _releaseLogic.GetById(releaseId);
            var containerName = _productLogic.GetById(release.ProductId).ContainerName + "-" + ((Malden.Portal.BLL.Release.ImageFileType)fileType).ToString().ToLower();
            var storageAccount = CloudStorageAccount.Parse(_releaseLogic.ReleaseConnectionKey());
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(containerName);
            var blockBlob = container.GetBlockBlobReference(fileName);
            var blobs = _blobLogic.CloudFiles(releaseId); //CloudFilesModel.FilteredFiles(releaseId, _releaseLogic, _productLogic, _blobLogic);
            var releaseArchive = new CloudFilesModel(container.ListBlobs(useFlatBlobListing: true), _blobLogic);
            var fileNameCount = releaseArchive.Files.Where(f => f.FileName == fileName).Count();
            var fileTypeExists = blobs.Where(f => f.ImageFileType == (Release.ImageFileType)fileType).Count();
            if (fileNameCount > 0)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            if (fileTypeExists > 0)
            {
                return Json(2, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFile(string fileName, string releaseId, int fileType)
        {
            var release = _releaseLogic.GetById(releaseId);
            var containerName = _productLogic.GetById(release.ProductId).ContainerName + "-" + ((Malden.Portal.BLL.Release.ImageFileType)fileType).ToString().ToLower();
            var storageAccount = CloudStorageAccount.Parse(_releaseLogic.ReleaseConnectionKey());
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(containerName);

            var blobs = _blobLogic.CloudFiles(releaseId);
            var blob = blobs.Where(b => b.ImageFileType == (Release.ImageFileType)fileType).FirstOrDefault();

            var blockBlob = container.GetBlockBlobReference(blob.FileName);

            try
            {
                blockBlob.Delete();
            }
            catch
            {

            }
            finally
            {

            }
            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            ViewBag.SelectedProductId = null;

            var releases = (from r in _releaseLogic.List()
                            select r).Select(r => new ReleaseViewModel
                                               {
                                                   ProductId = r.ProductId,
                                                   VersionString = r.Version.ToString(),
                                                   Id = r.Id,
                                                   ProductName = _productLogic.GetById(r.ProductId).Name,
                                                   Release = r,
                                                   ImageFileCount = _blobLogic.CountOfBlobFiles(r.Id),
                                                   IsHidden = r.IsHidden
                                               }).ToList();

            return View(releases.OrderBy(r => r.ProductName).ThenByDescending(r => r.VersionString));
        }

        public ActionResult IndexByProduct(int id)
        {
            var productId = _productLogic.GetById(id).Id;
            ViewBag.SelectedProductId = id;

            var releases = (from r in _releaseLogic.List()
                            where r.ProductId == productId
                            select r).Select(r => new ReleaseViewModel
                                               {
                                                   ProductId = r.ProductId,
                                                   VersionString = r.Version.ToString(),
                                                   Id = r.Id,
                                                   ProductName = _productLogic.GetById(r.ProductId).Name,
                                                   Release = r,
                                                   ImageFileCount = _blobLogic.CountOfBlobFiles(r.Id),
                                                   IsHidden = r.IsHidden
                                               }).ToList();

            return View("Index", releases.OrderBy(r => r.ProductName).ThenByDescending(r => r.VersionString));
        }

        public ActionResult ReleaseFiles(string id)
        {
            var values = from Malden.Portal.BLL.Release.ImageFileType e in
                             Enum.GetValues(typeof(Malden.Portal.BLL.Release.ImageFileType))
                         select new { Id = (int)e, Name = CustomHelpers.GetEnumDescription(e) };
            ViewBag.FileTypes = new SelectList(values, "Id", "Name");

            var productId = _productLogic.GetById(id).Id;

            var allFiles = new List<CloudFile>();

            var releases = (from r in _releaseLogic.List()
                            where r.ProductId == productId.ToString()
                            select r).Select(r => new ReleaseViewModel
                            {
                                Release = r,
                                VersionString = r.Version.ToString(),
                                IsHidden = r.IsHidden,
                                ImageFiles = _blobLogic.CloudFiles(r.Id)
                            }).ToList();

            ViewBag.ProductName = _productLogic.GetById(releases.FirstOrDefault().Release.ProductId).Name;

            return View(releases);
        }

        [HttpPost]
        public ActionResult SetMetadata(int blocksCount, string fileName, long fileSize, int fileIndex, int imageType, string releaseId)
        {
            var containerName = _releaseLogic.ContainerName((Malden.Portal.BLL.Release.ImageFileType)imageType, releaseId);
            var container = _blobLogic.GetContainer(containerName);
            var perm = new BlobContainerPermissions();
            perm.PublicAccess = BlobContainerPublicAccessType.Blob;
            container.SetPermissions(perm);

            var fileToUpload = new CloudFile()
            {
                BlockCount = blocksCount,
                FileName = fileName,
                Size = fileSize,
                BlockBlob = container.GetBlockBlobReference(fileName),
                StartTime = DateTime.Now,
                IsUploadCompleted = false,
                UploadStatusMessage = string.Empty,
                FileKey = "CurrentFile" + fileIndex.ToString(),
                FileIndex = fileIndex,
                ReleaseId = releaseId
            };
            fileToUpload.BlockBlob.Metadata.Add("ReleaseId", releaseId);
            Session.Add(fileToUpload.FileKey, fileToUpload);
            return Json(new { success = true, index = fileIndex });
        }

        public ActionResult Upload(string id)
        {
            var values = from Malden.Portal.BLL.Release.ImageFileType e in
                             Enum.GetValues(typeof(Malden.Portal.BLL.Release.ImageFileType))
                         select new
                         {
                             Id = (int)e,
                             Name = CustomHelpers.GetEnumDescription(e)
                         };

            ViewBag.FileTypes = new SelectList(values, "Id", "Name");

            var files = _blobLogic.CloudFiles(id); //CloudFilesModel.FilteredFiles(id, _releaseLogic, _productLogic, _blobLogic);

            var detailsOfRelease = _releaseLogic.GetById(id);

            ViewBag.PageTitle = "Files for " + _productLogic.GetById(detailsOfRelease.ProductId).Name + " - Version " + detailsOfRelease.Version.ToString();

            return View(files);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadChunk(int id, int fileIndex, string imageFileType)
        {
            HttpPostedFileBase request = Request.Files["Slice"];
            byte[] chunk = new byte[request.ContentLength];
            request.InputStream.Read(chunk, 0, Convert.ToInt32(request.ContentLength));
            JsonResult returnData = null;
            string fileSession = "CurrentFile" + fileIndex.ToString();

            if (Session[fileSession] != null)
            {
                CloudFile model = (CloudFile)Session[fileSession];
                returnData = UploadCurrentChunk(model, chunk, id);
                model.ImageFileType = (Release.ImageFileType)Convert.ToInt32(imageFileType);
                if (returnData != null)
                {
                    return returnData;
                }
                if (id == model.BlockCount)
                {
                    return CommitAllChunks(model);

                }
            }
            else
            {
                returnData = Json(new
                {
                    error = true,
                    isLastBlock = false,
                    message = string.Format(CultureInfo.CurrentCulture,
                        "Failed to Upload file.", "Session Timed out")
                });
                return returnData;
            }

            return Json(new { error = false, isLastBlock = false, message = string.Empty, index = fileIndex });
        }

        public ActionResult UploadFile()
        {
            return View();
        }

        private string GetDescription(Release.ImageFileType imageFileType)
        {
            var fileDescription = "";
            switch (imageFileType)
            {
                case Release.ImageFileType.Firmware: 
                    fileDescription = "Firmware";
                    break;
                case Release.ImageFileType.ISO: 
                    fileDescription = "Installer";
                    break;
                case Release.ImageFileType.MSI: 
                    fileDescription = "Upgrade";
                    break;
                case Release.ImageFileType.Notes: 
                    fileDescription = "Notes";
                    break;
                default:
                    fileDescription = "Unknown";
                    break;
            }

            return fileDescription;
        }

        private ActionResult CommitAllChunks(CloudFile model)
        {
            model.IsUploadCompleted = true;
            bool errorInOperation = false;
            try
            {
                var blockList = Enumerable.Range(1, (int)model.BlockCount).ToList<int>().ConvertAll(rangeElement =>
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                string.Format(CultureInfo.InvariantCulture, "{0:D4}", rangeElement))));
                model.BlockBlob.Metadata["ReleaseId"] = model.ReleaseId;
                model.BlockBlob.Properties.CacheControl = "public, max-age=" + WebConfigurationManager.AppSettings["BlobCacheControl"].ToString();
                var release = _releaseLogic.GetById(model.ReleaseId);
                var product = _productLogic.GetById(release.ProductId);

                model.BlockBlob.PutBlockList(blockList);

                var containerName = model.BlockBlob.Container.Name;
                var storageAccount = CloudStorageAccount.Parse(_releaseLogic.ReleaseConnectionKey());
                var storageClient = storageAccount.CreateCloudBlobClient();
                var container = storageClient.GetContainerReference(containerName);
                var blockBlob = container.GetBlockBlobReference(model.FileName);

                var newFileName = GetFileName(model, release, product);

                var newBlob = container.GetBlockBlobReference(newFileName);
                newBlob.StartCopyFromBlob(blockBlob);
                blockBlob.Delete();


                var duration = DateTime.Now - model.StartTime;
                float fileSizeInKb = model.Size / 1024;
                string fileSizeMessage = fileSizeInKb > 1024 ?
                    string.Concat((fileSizeInKb / 1024).ToString(CultureInfo.CurrentCulture), " MB") :
                    string.Concat(fileSizeInKb.ToString(CultureInfo.CurrentCulture), " KB");
                _blobLogic.RefreshBlobFileCount(model.ReleaseId);

                model.UploadStatusMessage = string.Format(CultureInfo.CurrentCulture,
                    "File uploaded successfully. {0} took {1} seconds to upload",
                    fileSizeMessage, Math.Round(duration.TotalSeconds, 2));
            }
            catch (StorageException e)
            {
                model.UploadStatusMessage = "Failed to Upload file. Exception - " + e.Message;
                errorInOperation = true;
            }
            finally
            {
                Session.Remove(model.FileKey);
            }
            return Json(new
            {
                error = errorInOperation,
                isLastBlock = model.IsUploadCompleted,
                message = model.UploadStatusMessage,
                index = model.FileIndex
            });
        }

        private string GetFileName(CloudFile model, Release release, Product product)
        {
            var lastDotlocation = model.FileName.LastIndexOf(".");
            var extension = "";
            if (lastDotlocation > 0)
            {
                extension = model.FileName.Substring(lastDotlocation + 1, model.FileName.Length - lastDotlocation - 1);
            }

            return product.Name + " v" + release.Version.ToString() + " " + GetDescription(model.ImageFileType) + "." + extension;
        }

        private JsonResult UploadCurrentChunk(CloudFile model, byte[] chunk, int id)
        {
            using (var chunkStream = new MemoryStream(chunk))
            {
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        string.Format(CultureInfo.InvariantCulture, "{0:D4}", id)));
                try
                {
                    model.BlockBlob.PutBlock(
                        blockId,
                        chunkStream, null, null,
                        new BlobRequestOptions()
                        {
                            RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(10), 3)
                        },
                        null);
                    return null;
                }
                catch (StorageException e)
                {
                    Session.Remove(model.FileKey);
                    model.IsUploadCompleted = true;
                    model.UploadStatusMessage = "Failed to Upload file. Exception - " + e.Message;
                    return Json(new { error = true, isLastBlock = false, message = model.UploadStatusMessage, index = model.FileIndex });
                }
            }
        }
    }
}