using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class CommandeEdit
    {
        public int id_com { get; set; }
        public Nullable<System.DateTime> date_commande { get; set; }
        public string id_users { get; set; }
        public bool livraison { get; set; }
    }
}