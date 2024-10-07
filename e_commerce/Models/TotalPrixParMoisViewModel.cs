using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class TotalPrixParMoisViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalPrice { get; set; }
    }
}