using Malden.Portal.Data;

namespace Malden.Portal.Tests.Releases
{
    public class TestRelease : IRelease
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public string Version { get; set; }

        public System.DateTime DateOfRelease { get; set; }

        public string FileBlockReference { get; set; }

        public string ImageFile { get; set; }

        public bool IsHidden { get; set; }
    }
}