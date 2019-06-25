using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Product
    {
        //Primary Key
        public int Id { get; set; }
        [Required]
        //Foreign Key For the type of product the product is
        public int ProductTypeId { get; set; }
        [Required]
        //Foreign Key for the customer that is ordering this product
        public int CustomerId { get; set; }
        [Required]
        //Price of the product
        public decimal Price { get; set; }
        [Required]
        //Product name
        public string Title { get; set; }
        [Required]
        //Description of what the product is
        public string Description { get; set; }
        [Required]
        //Number of those products being ordered
        public int Quantity { get; set; }
    }
}
