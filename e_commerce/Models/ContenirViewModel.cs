using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class ContenirViewModel
    {
        public string id_user { get; internal set; }
        public int id_panier { get; internal set; }
        public string id_prod { get; internal set; }
        public string nom_prod { get; internal set; }
        public int qte { get; internal set; }
        public int prix { get; internal set; }
        public int prix_prod { get; internal set; }
    }
}