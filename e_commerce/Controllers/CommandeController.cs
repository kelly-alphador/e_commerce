using e_commerce.Data;
using e_commerce.Logic;
using e_commerce.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace e_commerce.Controllers
{
    public class CommandeController : Controller
    {
        public ActionResult ConfirmationCommande(int idCommande)
        {
            // Assigner une valeur au ViewBag pour éviter l'erreur de référence null
            ViewBag.Title = "Confirmation de la Commande";

            // Vous pouvez également passer des données supplémentaires à la vue, si nécessaire
            ViewBag.IdCommande = idCommande;

            return View();
        }
        public ActionResult GenererPdfFacture(int idCommande)
        {
            using (var context = new E_COMMERCEEntities())
            {
                // Récupérer la commande par son ID
                var command = context.COMMANDE.FirstOrDefault(c => c.id_com == idCommande);

                if (command == null)
                {
                    return RedirectToAction("ErreurCommande"); // Redirection si la commande n'existe pas
                }

                // Récupérer le panier lié à cette commande
                var panier = context.PANIER.FirstOrDefault(p => p.id_user == command.id_users);
                if (panier == null)
                {
                    return RedirectToAction("ErreurCommande"); // Redirection si aucun panier n'est trouvé
                }

                // Récupérer les produits associés au panier
                var produits = (from pp in context.CONTENIR
                                join p in context.PRODUIT on pp.id_prod equals p.id_prod
                                join u in context.USERS on panier.id_user equals u.id_user
                                where pp.id_panier == panier.id_panier // Utiliser le panier lié
                                select new
                                {
                                    NomProduit = p.nom,
                                    Quantite = pp.qte,
                                    PrixUnitaire = p.prix,
                                    Total = pp.qte * p.prix,
                                    nomUser=u.nom,
                                    telephone=u.telephone,
                                    adresse=u.adresse,
                                }).ToList();
             
                if (produits.Count == 0)
                {
                    return RedirectToAction("ErreurCommande"); // Redirection si aucun produit trouvé
                }

                // Création du PDF
                var document = new Document();
                MemoryStream stream = new MemoryStream();
                PdfWriter.GetInstance(document, stream).CloseStream = false;

                document.Open();
                // Titre centré
                Paragraph titre = new Paragraph("Facture de Commande");
                titre.Alignment = Element.ALIGN_CENTER;
                document.Add(titre);
                document.Add(new Paragraph("Commande ID : " + command.id_com));
                document.Add(new Paragraph("Nom du Client : " + produits[0].nomUser));
                document.Add(new Paragraph("Date : " + command.date_commande?.ToString("dd/MM/yyyy") ?? "Date non disponible"));
                document.Add(new Paragraph("Telephone : " + produits[0].telephone ));
                document.Add(new Paragraph("adresse : " + produits[0].adresse));
                // Ajouter un espace avant la table
                document.Add(new Paragraph("\n"));
                PdfPTable table = new PdfPTable(4);
                table.AddCell("Produit");
                table.AddCell("Prix Unitaire");
                table.AddCell("Quantité");
                table.AddCell("Total");

                foreach (var produit in produits)
                {
                    table.AddCell(produit.NomProduit);
                    table.AddCell(produit.PrixUnitaire.ToString());
                    table.AddCell(produit.Quantite.ToString());
                    table.AddCell(produit.Total.ToString());
                }

                document.Add(table);
                // Ajouter un autre espace si besoin
                document.Add(new Paragraph("\n"));
                // Calculer le total général
                var totalGeneral = produits.Sum(p => p.Total);
                // Total général aligné à droite
                Paragraph total = new Paragraph("TOTAL : " + totalGeneral + " Ariary");
                total.Alignment = Element.ALIGN_RIGHT;
                document.Add(total);

                document.Close();

                // Retourner le PDF en tant que fichier téléchargeable
                ViderPanier();
                return File(stream.ToArray(), "application/pdf", "Facture_Commande_" + idCommande + ".pdf");
            }
        }

        public ActionResult PasserCommande()
        {
            using (var context = new E_COMMERCEEntities())
            {
                // Récupérer l'utilisateur courant
                var idUser = User.Identity.GetUserId();

                // Créer une instance du gestionnaire de panier
                var pannierManager = new PannierManager();

                // Récupérer l'ID du panier pour l'utilisateur courant
                int idPanier = pannierManager.RecupererIdPanier(idUser);

                // Récupérer le panier pour cet utilisateur à partir de l'ID du panier
                var panier = (from pp in context.CONTENIR
                              join p in context.PRODUIT on pp.id_prod equals p.id_prod
                              where pp.id_panier == idPanier
                              select new ProduitPanierViewModel
                              {
                                  id_prod = p.id_prod,
                                  nom = p.nom,
                                  qte = pp.qte,
                                  prix = p.prix
                              }).ToList();

                // Vérifier si le panier est vide
                if (panier == null || panier.Count == 0)
                {
                    // Gérer le cas où le panier est vide
                    return RedirectToAction("PanierVide");
                }

                // Créer la commande
                var commande = new COMMANDE
                {
                    date_commande = DateTime.Now,
                    id_users = idUser,
                };
                context.COMMANDE.Add(commande);
                context.SaveChanges();

                // Sauvegarder l'ID de la commande nouvellement créée
                var idCommande = commande.id_com;

                // Parcourir les produits dans le panier
                foreach (var produitPanier in panier)
                {
                    var produit = context.PRODUIT.FirstOrDefault(p => p.id_prod == produitPanier.id_prod);

                    if (produit != null)
                    {
                        if (produit.qte >= produitPanier.qte)
                        {
                            produit.qte -= produitPanier.qte;
                            context.Entry(produit).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Quantité insuffisante pour le produit : {produit.nom}";
                        }
                    }
                }

                context.SaveChanges();

                Session["Panier"] = null;

                // Passer l'ID de la commande à la vue ConfirmationCommande
               // return RedirectToAction("ConfirmationCommande", new { idCommande });
                return RedirectToAction("ConfirmationCommande", new { idCommande = idCommande });
            }
        }

        public JsonResult ViderPanier()
        {
            var idUser = User.Identity.GetUserId();
            using (var context = new E_COMMERCEEntities())
            {
                var EntityPanier = context.PANIER.FirstOrDefault(p => p.id_user == idUser);
                if (EntityPanier != null)
                {
                    var ContenuPanier = context.CONTENIR
                        .Where(c => c.id_panier == EntityPanier.id_panier)
                        .ToList();

                    context.CONTENIR.RemoveRange(ContenuPanier);
                    context.SaveChanges();

                    // Calculer la nouvelle quantité totale dans le panier
                    var totalItems = 0; // Puisque le panier est vide après cette opération

                    // Mettre à jour la session
                    Session["SommeQuantitePanier"] = totalItems;

                    return Json(new { success = true, message = "Panier vidé avec succès.", totalItems });
                }

                return Json(new { success = false, message = "Panier introuvable." });
            }
        }
    }

}