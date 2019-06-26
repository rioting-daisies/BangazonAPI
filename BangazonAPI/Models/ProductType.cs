// Author: Billy Mitchell
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models    
{
    // The purpose of the ProductType class is to be a Model which represents the ProductType table in the BangazonAPI Database 
    public class ProductType
    {   // Gets and Sets the identity integer for a single object in the ProductType table and becomes the primary key for each ProductType instance
        public int Id { get; set; }
        // This property is used to get and set the name of the category
        [Required]
        public string Name { get; set; }
    }
}
