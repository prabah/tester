using System;
using System.ComponentModel.DataAnnotations;

namespace Malden.Portal.BLL
{
    public class Product
    {
        public Product(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "Please enter a valid Name for the software");

            Name = name;
        }

        public Product()
        { }

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Fulfilment Id")]
        public int FulfilmentId { get; set; }

        [Required]
        [Display(Name = "Container/Directory Name")]
        [MinLength(5)]
        public string ContainerName { get; set; }

        [Display(Name = "Maintained Product")]
        public bool IsMaintained { get; set; }
    }
}