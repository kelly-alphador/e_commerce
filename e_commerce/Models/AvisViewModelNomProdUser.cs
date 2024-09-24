using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class AvisViewModelNomProdUser
    {
        public string commentaire { get; internal set; }
        public double Note { get; internal set; }
        public DateTime DateAvis { get; internal set; }
        public string Nomuser { get; internal set; }
        public string NomProd { get; internal set; }
        public int idAvis { get; internal set; }
    }
}