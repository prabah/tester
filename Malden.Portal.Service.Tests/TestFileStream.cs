using Malden.Portal.Service.WebNew;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Malden.Portal.Service.Tests
{
    [TestClass]
    public class TestFileStream
    {
        [TestMethod]
        public void it_can_validate_serialnumber()
        {
            var maldenStream = new MaldenStream();
            var resultFile = maldenStream.ImageFile("prabah@malden.co.uk1", 2822);

            var ms = new MemoryStream();
            resultFile.CopyTo(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            resultFile.Close();
            //return File(ms, "application/zip", string.Format("{0}.txt", "file"));
        }

        [TestMethod]
        public void it_can_download_old_release()
        {
            var maldenStream = new MaldenStream();
            var resultFile = maldenStream.OldRelease("prabah@malden.co.uk", "2f1a1d5d", 2822);

            var ms = new MemoryStream();
            resultFile.CopyTo(ms);
            //ms.Seek(0, SeekOrigin.Begin);
            resultFile.Close();
            //return File(ms, "application/zip", string.Format("{0}.txt", "file"));
        }
    }
}