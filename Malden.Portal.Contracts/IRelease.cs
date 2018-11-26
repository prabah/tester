using System;

namespace Malden.Portal.Data
{
    public interface IRelease
    {
        string Id { get; set; }

        string ProductId { get; set; }

        string Version { get; set; }

        string ImageFile { get; set; }

        DateTime DateOfRelease { get; set; }

        bool IsHidden { get; set; }
    }
}