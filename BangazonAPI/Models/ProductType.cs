using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class ProductType
    {
        [Required]
        public int Id { get; set;  }
        [Required]
        public string Name { get; set;  }
    }
}

//class created by Alex