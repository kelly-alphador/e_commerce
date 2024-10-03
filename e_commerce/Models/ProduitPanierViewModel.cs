using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class ProduitPanierViewModel
    {
        public string id_prod { get; set; }
        public string nom { get; set; }
        public int qte { get; set; }
        public int prix { get; set; }

        // Calculer le total pour un produit (quantité * prix)
        public int Total { get { return qte * prix; } }
    }
}