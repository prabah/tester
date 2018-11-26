using System;

namespace Malden.Portal.Data
{
    public interface IMaintenanceContract
    {
        string Id { get; set; }

        string SerialKeyId { get; set; }

        DateTime DateOfExpiry { get; set; }

        int OrderId { get; set; }
    }
}