using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class ProduitEdite
    {
        public string id_prod { get; set; }
        public string nom { get; set; }
        public string description { get; set; }
        public int prix { get; set; }
        public int qte { get; set; }
        public string ImageUrl { get; set; }
        public string id_categorie { get; set; }
    }
}