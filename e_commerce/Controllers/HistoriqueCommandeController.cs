using e_commerce.Data;
using e_commerce.Filters;
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
        [Authorize]
        public ActionResult historique_commande_client()
        {
            var idUser = User.Identity.GetUserId();
            using (var context = new E_COMMERCEEntities())
            {
                var historiqueCommandes = context.COMMANDE
                    .Where(c => c.id_users == idUser)
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
                            NomProduit = cdp.Produit.nom, // Nom du produit
                    Quantite = cdp.DetailCommande.qte, // Quantité
                    PrixUnitaire = cdp.DetailCommande.prix_unitaire, // Prix unitaire
                    SousTotal = cdp.DetailCommande.qte * cdp.DetailCommande.prix_unitaire,
                            StatutLivraison = cdp.Commande.livraison // Ajouter le champ statut de livraison
                })
                    .ToList() // Récupérer les données pour traitement en mémoire
                    .GroupBy(x => new
                    {
                        x.IdCom,
                        x.DateCommande,
                        x.NomClient,
                        x.StatutLivraison // Ajouter le statut dans le groupement
            })
                    .Select(g => new HistoriqueCommandeViewModel
                    {
                        IdCom = g.Key.IdCom,
                        DateCommande = g.Key.DateCommande,
                        NomClient = g.Key.NomClient,
                        TotalCommande = g.Sum(x => x.SousTotal),
                        StatutLivraison = g.Key.StatutLivraison ? "Déjà livré" : "Non livré", // Mapping du statut
                Details = g.Select(x => new DetailCommandeViewModel
                        {
                            NomProduit = x.NomProduit,
                            Quantite = x.Quantite,
                            PrixUnitaire = x.PrixUnitaire,
                            SousTotal = x.SousTotal
                        }).ToList()
                    })
                    .OrderByDescending(x => x.DateCommande)
                    .ToList();

                return View(historiqueCommandes);
            }
        }



        [HttpGet]
        public ActionResult Edite(int id)
        {
            using(var context = new E_COMMERCEEntities())
            {
                COMMANDE commande = context.COMMANDE.Single(c => c.id_com==id);
                var CommandeEdit = new CommandeEdit
                {
                    id_com = commande.id_com,
                    date_commande=commande.date_commande,
                    id_users=commande.id_users,
                    livraison=commande.livraison
                };
                return View(CommandeEdit);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edite(CommandeEdit commandeEdit)
        {
            if (ModelState.IsValid)
            {
                using (var context = new E_COMMERCEEntities())
                {
                    var commande = context.COMMANDE.Single(c => c.id_com == commandeEdit.id_com);

                    // Mise à jour des propriétés
                    commande.date_commande = commandeEdit.date_commande;
                    commande.id_users = commandeEdit.id_users;
                    commande.livraison = commandeEdit.livraison;

                    context.SaveChanges();

                    // Ajouter un message de succès dans TempData
                    TempData["SuccessMessageEdite"] = "La commande a été sauvegardée avec succès.";
                }

                return RedirectToAction("historique_commande_admin");
            }

            return View(commandeEdit);
        }


        [AdminAuthorize]
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
                            TotalCommande = cdp.DetailCommande.qte * cdp.DetailCommande.prix_unitaire,
                            Livraison = cdp.Commande.livraison // Ajout du champ livraison
                })
                    .GroupBy(x => new
                    {
                        x.IdCom,
                        x.DateCommande,
                        x.NomClient,
                        x.Livraison
                    })
                    .Select(g => new HistoriqueCommandeViewModel
                    {
                        IdCom = g.Key.IdCom,
                        DateCommande = g.Key.DateCommande,
                        NomClient = g.Key.NomClient,
                        TotalCommande = g.Sum(x => x.TotalCommande),
                        StatutLivraison = g.Key.Livraison ? "Déjà livré" : "Non livré" // Mapping du statut
            })
                    .OrderByDescending(x => x.DateCommande)
                    .ToList();

                return View(historiqueCommandes);
            }
        }

    }
}