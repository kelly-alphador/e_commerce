using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class TableauViewModel
    {
        public int TotalClients { get; set; }
        public int TotalCommandes { get; set; }
        public int TotalProduits { get; set; }
        public List<ProduitVenteViewModel> CinqProduitsLesPlusVendus { get; set; }
        public List<ClientFideleViewModel> CinqClientsLesPlusFideles { get; set; }
        public List<TotalPrixParMoisViewModel> TotalPrixParMois { get; set; }
        public int? TotalPrixCommandes { get; internal set; }
    }
    public class ProduitVenteViewModel
    {
        public string NomProduit { get; set; }
        public int QuantiteVendue { get; set; }
    }

    public class ClientFideleViewModel
    {
        public string NomClient { get; set; }
        public int NombreCommandes { get; set; }
    }
}