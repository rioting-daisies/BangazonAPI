// Author Chris Morgan

// The purpose of the Customer class is to be a Model that represents the Customer table within the BangazonAPI Database. This allows us to use local instances of a Customer within our C#

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Customer
    {

        // The Id property is a public int that represents the Primary Key of a Customer item in the Database
        public int Id { get; set; }

        // The FirstName property is a public string that is required when the Customer is created. This represents the first name of the customer
        [Required]
        public string FirstName { get; set; }

        // The LastName property is a public string that is required when the Customer is created. This represents the last name of the customer
        [Required]
        public string LastName { get; set; }

        public List<Product> ListOfProducts { get; set; } = new List<Product>();
    }
}