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

        public ActionResult Index()
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
        }
    }
}
