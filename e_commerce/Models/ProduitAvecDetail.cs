using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class ProduitAvecDetail
    {

        public string nom { get; internal set; }
        public string Description { get; internal set; }
        public int prix { get; internal set; }
        public int qte { get; internal set; }
        public string ImageUrl { get; internal set; }
    }
}