using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class HistoriqueCommandeViewModel
    {
        public int IdCom { get; internal set; }
        public DateTime? DateCommande { get; internal set; }
        public string NomClient { get; internal set; }
        public int? TotalCommande { get; internal set; }
    }
}