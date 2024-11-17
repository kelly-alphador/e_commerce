using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class DetailCommandeViewModel
    {
        public string NomProduit { get; set; }
        public int? Quantite { get; set; }
        public int? PrixUnitaire { get; set; }
        public int? SousTotal { get; set; }
    }
}