// Author: Billy Mitchell
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
    
{   // This class represents the category for which a product is related to, such as: Appliances or TVs.
    public class ProductType
    {   // Primary Key
        public int Id { get; set; }
        // This property is used to get and set the name of the category
        [Required]
        public string Name { get; set; }
    }
}
