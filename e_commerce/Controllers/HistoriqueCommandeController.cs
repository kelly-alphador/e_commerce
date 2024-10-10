using e_commerce.Data;
using e_commerce.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_commerce.Controllers
{
    public class HistoriqueCommandeController : Controller
    {
        // GET: HistoriqueCommande
        public ActionResult historique_commande_client()
        {
            var idUser = User.Identity.GetUserId();
            using (var context = new E_COMMERCEEntities())
            {
                var historiqueCommandes = context.COMMANDE.Where(c => c.id_users == idUser)
                   .Join(context.DETAIL_COMMANDE,
                       c => c.id_com,
                       dc => dc.id_com,
                       (c, dc) => new { Commande = c, DetailCommande = dc })
                   .Join(context.PRODUIT,
                       cd => cd.DetailCommande.id_prod,
                       p => p.id_prod,
                       (cd, p) => new { cd.Commande, cd.DetailCommande, Produit = p })
                   .Join(context.USERS,
                       cdp => cdp.Commande.id_users,
                       u => u.id_user,
                       (cdp, u) => new
                       {
                           IdCom = cdp.Commande.id_com,
                           DateCommande = cdp.Commande.date_commande,
                           NomClient = u.nom,
                           TotalCommande = cdp.DetailCommande.qte * cdp.DetailCommande.prix_unitaire
                       })
                   .GroupBy(x => new
                   {
                       x.IdCom,
                       x.DateCommande,
                       x.NomClient
                   })
                   .Select(g => new HistoriqueCommandeViewModel
                   {
                       IdCom = g.Key.IdCom,
                       DateCommande = g.Key.DateCommande,
                       NomClient = g.Key.NomClient,
                       TotalCommande = g.Sum(x => x.TotalCommande)
                   })
                   .OrderByDescending(x => x.DateCommande)
                   .ToList();

                return View(historiqueCommandes);

            }
        }
        public ActionResult historique_commande_admin()
        {
            using (var context = new E_COMMERCEEntities())
            {
                var historiqueCommandes = context.COMMANDE
                    .Join(context.DETAIL_COMMANDE,
                        c => c.id_com,
                        dc => dc.id_com,
                        (c, dc) => new { Commande = c, DetailCommande = dc })
                    .Join(context.PRODUIT,
                        cd => cd.DetailCommande.id_prod,
                        p => p.id_prod,
                        (cd, p) => new { cd.Commande, cd.DetailCommande, Produit = p })
                    .Join(context.USERS,
                        cdp => cdp.Commande.id_users,
                        u => u.id_user,
                        (cdp, u) => new
                        {
                            IdCom = cdp.Commande.id_com,
                            DateCommande = cdp.Commande.date_commande,
                            NomClient = u.nom,
                            TotalCommande = cdp.DetailCommande.qte * cdp.DetailCommande.prix_unitaire
                        })
                    .GroupBy(x => new
                    {
                        x.IdCom,
                        x.DateCommande,
                        x.NomClient
                    })
                    .Select(g => new HistoriqueCommandeViewModel
                    {
                        IdCom = g.Key.IdCom,
                        DateCommande = g.Key.DateCommande,
                        NomClient = g.Key.NomClient,
                        TotalCommande = g.Sum(x => x.TotalCommande)
                    })
                    .OrderByDescending(x => x.DateCommande)
                    .ToList();

                return View(historiqueCommandes);
            }
        }

    }
}