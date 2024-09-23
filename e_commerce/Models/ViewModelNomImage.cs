using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class ViewModelNomImage
    {
        public string nom { get; internal set; }
        public string imageUrl { get; internal set; }
        public int nombreAvis { get; internal set; }
        public string id { get; internal set; }
    }
}