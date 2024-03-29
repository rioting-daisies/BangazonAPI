﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int MaxAttendees { get; set; }
        public List<Employee> employees { get; set; } = new List<Employee>();
        //public string IsComplete()
        //{
        //    DateTime now = DateTime.Now;
        //    if(StartDate >= now)
        //    {
        //        return "false";
        //    } else
        //    {
        //        return "true";
        //    }
        //}
    }
}
