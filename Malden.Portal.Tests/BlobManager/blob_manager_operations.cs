using Malden.Portal.BLL;
using Malden.Portal.GUI.Azure.Webrole.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;
using System;
using System.IO;

namespace Malden.Portal.Tests.BlobManager
{
    [TestClass]
    public class blob_manager_operations
    {
        private readonly StandardKernel _kernel = new StandardKernel();
        private IBlobManagerLogic _blobManagerLogic;
        private IReleaseLogic _releaseLogic;
        

        [TestMethod]
        public void RefreshBlobCount()
        {
            Setup();
            
            _blobManagerLogic.RefreshBlobFileCount("0167fd56-7e0a-446c-b1c5-26f61190731a");
        }

        [TestMethod]
        public void can_get_count_of_files()
        {
            Setup();
            
            var count = _blobManagerLogic.CountOfBlobFiles("0167fd56-7e0a-446c-b1c5-26f61190731a");
        }


        [TestMethod]
        public void it_can_update_all_release_count()
        {
            Setup();
            var releases = _releaseLogic.List();

            foreach (var release in releases)
            {
                _blobManagerLogic.RefreshBlobFileCount(release.Id);
            }
        }

        private void Setup()
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);
            _blobManagerLogic = _kernel.Get<IBlobManagerLogic>();
            _releaseLogic = _kernel.Get<IReleaseLogic>();
        }

        [TestMethod]
        public void it_can_download_multiple_files()
        {
            var url = "https://swmel.blob.core.windows.net/multidsla-msi/mdslainst 4.6.0.msi";

            var startTime = DateTime.Now;
            var parellel = new PrallelBlobTransfer(new Microsoft.WindowsAzure.StorageClient.CloudBlobContainer("https://swmel.blob.core.windows.net:443/multidsla"));
            parellel.ParallelDownloadFile(new FileStream("C:\\sample\\sdfsd.iso",FileMode.Create), url);

            var diff = (DateTime.Now - startTime).Seconds;
            //parellel.DownloadBlobToBufferAsync(new byte[255], url);
        }
        
    }
}
