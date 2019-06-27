// Author: Billy Mitchell
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    // The purpose of the order class is to be a Model which represents the order table in the BangazonAPI Database 
    public class Order
    {   
        // Gets and Sets the identity integer for a single object in the order table and becomes the primary key for each order
        public int Id { get; set; }
        // Gets and Sets the Customer who is associated with the order by the customer's Id
        [Required]
        public int CustomerId { get; set; }
        // Gets and sets the customer's payment type through the Id associated with their payment account.
        public int? PaymentTypeId { get; set; }
        // Creates a list to for products
        public List<Product> ListOfProducts { get; set; } = new List<Product>();
    }
}
