using e_commerce.Data;
using e_commerce.Models; // Importer le modèle
using System;
using System.Linq;
using System.Web.Mvc;

namespace e_commerce.Controllers
{
    public class TableauController : Controller
    {
        private E_COMMERCEEntities db = new E_COMMERCEEntities();

        /*public ActionResult Index()
        {
            // Calculer le total des prix des commandes passées
            var totalPrixCommandes = db.DETAIL_COMMANDE
                .Sum(dc => dc.qte * dc.prix_unitaire);

            // Calculer la date cible pour les 12 derniers mois
            DateTime dateCible = DateTime.Now.AddMonths(-12);

            // Calculer le total des prix des commandes pour chaque mois des 12 derniers mois
            var totalPrixParMois = db.COMMANDE
                .Where(c => c.date_commande.HasValue && c.date_commande.Value > dateCible)
                .Join(db.DETAIL_COMMANDE, c => c.id_com, dc => dc.id_com, (c, dc) => new { c.date_commande, dc.qte, dc.prix_unitaire })
                .GroupBy(x => new { Year = x.date_commande.Value.Year, Month = x.date_commande.Value.Month })
                .Select(g => new TotalPrixParMoisViewModel // Utiliser le ViewModel ici
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalPrice = (int)g.Sum(x => x.qte * x.prix_unitaire)
                })
                .ToList();

            // Passer les données via ViewBag
            ViewBag.TotalPrixCommandes = totalPrixCommandes;
            ViewBag.TotalPrixParMois = totalPrixParMois;

            return View();
        }*/
        public ActionResult Index()
        {
            var viewModel = new TableauViewModel();

            // Calculer le total des prix des commandes passées
            viewModel.TotalPrixCommandes = db.DETAIL_COMMANDE.Sum(dc => dc.qte * dc.prix_unitaire);
            // Calculer le nombre total de clients
            viewModel.TotalClients = db.USERS.Count();

            // Calculer le nombre total de commandes
            viewModel.TotalCommandes = db.COMMANDE.Count();

            // Calculer le nombre total de produits
            viewModel.TotalProduits = db.PRODUIT.Count();

            // Calculer la date cible pour les 12 derniers mois
            DateTime dateCible = DateTime.Now.AddMonths(-12);

            // Calculer le total des prix des commandes pour chaque mois des 12 derniers mois
            viewModel.TotalPrixParMois = db.COMMANDE
                .Where(c => c.date_commande.HasValue && c.date_commande.Value > dateCible)
                .Join(db.DETAIL_COMMANDE, c => c.id_com, dc => dc.id_com, (c, dc) => new { c.date_commande, dc.qte, dc.prix_unitaire })
                .GroupBy(x => new { Year = x.date_commande.Value.Year, Month = x.date_commande.Value.Month })
                .Select(g => new TotalPrixParMoisViewModel
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalPrice = (int)g.Sum(x => x.qte * x.prix_unitaire)
                })
                .ToList();

            // Obtenir les 5 produits les plus vendus
            viewModel.CinqProduitsLesPlusVendus = db.DETAIL_COMMANDE
                .GroupBy(dc => new { dc.id_prod, dc.PRODUIT.nom }) // Assurez-vous que nom existe dans votre modèle Produit
                .Select(g => new ProduitVenteViewModel
                {
                    NomProduit = g.FirstOrDefault().PRODUIT.nom,
                    QuantiteVendue = (int)g.Sum(dc => dc.qte)
                })
                .OrderByDescending(x => x.QuantiteVendue)
                .Take(5)
                .ToList();

            // Obtenir les 5 clients les plus fidèles
            viewModel.CinqClientsLesPlusFideles = db.COMMANDE
                .GroupBy(c => new { c.id_users, c.USERS.nom }) // Assurez-vous que nom existe dans votre modèle Client
                .Select(g => new ClientFideleViewModel
                {
                    NomClient = g.FirstOrDefault().USERS.nom,
                    NombreCommandes = g.Count()
                })
                .OrderByDescending(x => x.NombreCommandes)
                .Take(5)
                .ToList();
          
            // Passer le ViewModel à la vue
            return View(viewModel);
        }

    }
}
