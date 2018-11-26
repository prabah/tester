using System;

namespace Malden.Portal.BLL
{
    public class ProductCatalogue
    {
        public ProductCatalogue()
        { }

        public ProductCatalogue(int serialNumber)
        {
            if (serialNumber <= 0) throw new ArgumentNullException("serialNumber", "Serial key is a required field");
            SerialNumber = serialNumber;
        }

        public string Id { get; set; }

        public string ProductId { get; set; }

        public int SerialNumber { get; set; }

        public string CurrentRelease { get; set; }
    }
}