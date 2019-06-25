using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class PaymentType
    {
        public int Id { get; set; }
        [Required]
        public int AcctNumber { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CustomerId { get; set; }
    }
}

//author Alex. class has same properties as SQL data.  CustomerId references Customer table to show what paymentType is with different customers as a 1 to many relationship