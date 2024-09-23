using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class AvisViewModel
    {
        public string Commentaire { get; internal set; }
        public double Note { get; internal set; }
        public string NomPersonne { get; internal set; }
        public string id_produit { get; internal set; }
    }
}