
using System;
namespace Malden.Portal.Data
{
    public interface IEmailManager
    {
        string Id { get; set; }
        string UserId { get; set; }
        bool Activated { get; set; }
        int EmailType { get; set; }
        DateTime ActivatedTime { get; set; } 
    }
}
