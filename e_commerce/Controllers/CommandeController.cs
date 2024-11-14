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
using iTextSharp.text.pdf.draw;
using Humanizer;

namespace e_commerce.Controllers
{
    public class CommandeController : Controller
    {
        public ActionResult ConfirmationCommande(int idCommande)
        {
            using (var context = new E_COMMERCEEntities())
            {
                var command = context.COMMANDE.FirstOrDefault(c => c.id_com == idCommande);

                if (command == null)
                {
                    return RedirectToAction("ErreurCommande");
                }

                // Récupérer les produits associés à la commande
                var produits = (from dc in context.DETAIL_COMMANDE
                                join p in context.PRODUIT on dc.id_prod equals p.id_prod
                                where dc.id_com == command.id_com
                                select new
                                {
                                    NomProduit = p.nom,
                                    Quantite = dc.qte,
                                    PrixUnitaire = dc.prix_unitaire,
                                    Total = dc.qte * dc.prix_unitaire
                                }).ToList();

                if (produits.Count == 0)
                {
                    return RedirectToAction("ErreurCommande");
                }

                var totalGeneral = (decimal)produits.Sum(p => p.Total);
                var totalEnEuro = ConvertirEnEuro(totalGeneral);

                // Assigner les valeurs au ViewBag pour qu'elles soient disponibles dans la vue
                ViewBag.IdCommande = idCommande;
                ViewBag.TotalEnEuro = totalEnEuro;

                return View();
            }
        }


     
        public ActionResult GenererPdfFacture(int idCommande)
        {
            using (var context = new E_COMMERCEEntities())
            {
                var command = context.COMMANDE.FirstOrDefault(c => c.id_com == idCommande);

                if (command == null)
                {
                    return RedirectToAction("ErreurCommande");
                }

                var produits = (from dc in context.DETAIL_COMMANDE
                                join p in context.PRODUIT on dc.id_prod equals p.id_prod
                                join u in context.USERS on command.id_users equals u.id_user
                                where dc.id_com == command.id_com
                                select new
                                {
                                    NomProduit = p.nom,
                                    Quantite = dc.qte,
                                    PrixUnitaire = dc.prix_unitaire,
                                    Total = dc.qte * dc.prix_unitaire,
                                    nomUser = u.nom,
                                    telephone = u.telephone,
                                    adresse = u.adresse,
                                }).ToList();

                if (produits.Count == 0)
                {
                    return RedirectToAction("ErreurCommande");
                }

                var document = new Document();
                MemoryStream stream = new MemoryStream();
                PdfWriter.GetInstance(document, stream).CloseStream = false;

                document.Open();

                // Titre centré et coloré en bleu
                Paragraph titre = new Paragraph("Facture de Commande")
                {
                    Alignment = Element.ALIGN_CENTER,
                    Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLUE) // Changer la couleur ici
                };
                document.Add(titre);

                // Ligne horizontale
                document.Add(new Paragraph(new Chunk(new LineSeparator())));

                // Ajouter une image à côté des informations client
                string imagePath = Server.MapPath("~/content/Images/spray_info.jpg"); // Chemin vers l'image
                Image logo = Image.GetInstance(imagePath);
                logo.ScaleToFit(130f, 130f); // Redimensionner si nécessaire

                // Table pour afficher l'image et les informations client côte à côte
                PdfPTable clientInfoTable = new PdfPTable(2);
                clientInfoTable.WidthPercentage = 100; // Vous pouvez ajuster la largeur ici si nécessaire

                // Colonne pour les informations client
                PdfPCell cellInfo = new PdfPCell();
                cellInfo.Border = Rectangle.NO_BORDER;
                cellInfo.AddElement(new Paragraph("Commande ID : " + command.id_com));
                cellInfo.AddElement(new Paragraph("Nom du Client : " + produits[0].nomUser));
                cellInfo.AddElement(new Paragraph("Date : " + command.date_commande?.ToString("dd/MM/yyyy") ?? "Date non disponible"));
                cellInfo.AddElement(new Paragraph("Telephone : " + produits[0].telephone));
                cellInfo.AddElement(new Paragraph("Adresse : " + produits[0].adresse));
                clientInfoTable.AddCell(cellInfo);

                // Colonne pour l'image
                PdfPCell cellImage = new PdfPCell(logo);
                cellImage.Border = Rectangle.NO_BORDER;
                clientInfoTable.AddCell(cellImage);

                // Ajouter le tableau des informations client
                document.Add(clientInfoTable);
                document.Add(new Paragraph("\n"));

                // Table des produits
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100; // Ajustez la largeur de la table des produits
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
                document.Add(new Paragraph("\n"));

                var totalGeneral = (decimal)produits.Sum(p => p.Total);

                // Afficher le total en Ariary
                Paragraph totalAriary = new Paragraph("TOTAL : " + totalGeneral + " Ariary");
                totalAriary.Alignment = Element.ALIGN_RIGHT;
                document.Add(totalAriary);
                //Afficher le chiffre en lettre
                int chiffre_lettre = (int)totalGeneral;
                Paragraph totalLettre = new Paragraph("TOTAL : " + chiffre_lettre.ToWords() + " Ariary");
                totalLettre.Alignment = Element.ALIGN_CENTER;
                document.Add(totalLettre);

                document.Close();

                stream.Position = 0;
                Response.AppendHeader("Content-Disposition", "inline; filename=Facture_Commande_" + idCommande + ".pdf");

                return File(stream, "application/pdf");
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

                // Récupérer les produits du panier
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
                    return RedirectToAction("PanierVide");
                }

                // Créer la commande
                var commande = new COMMANDE
                {
                    date_commande = DateTime.Now,
                    id_users = idUser
                };
                context.COMMANDE.Add(commande);
                context.SaveChanges();  // Sauvegarder pour obtenir l'ID de la commande

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
                            // Mise à jour de la quantité du produit dans la base de données
                            produit.qte -= produitPanier.qte;
                            context.Entry(produit).State = System.Data.Entity.EntityState.Modified;

                            // Ajouter les articles à la commande
                            context.DETAIL_COMMANDE.Add(new DETAIL_COMMANDE
                            {
                                id_com = idCommande,
                                id_prod = produitPanier.id_prod,
                                qte = produitPanier.qte,
                                prix_unitaire = produitPanier.prix
                            });
                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Quantité insuffisante pour le produit : {produit.nom}";
                            return RedirectToAction("Panier");
                        }
                    }
                }

                // Sauvegarder les détails de la commande
                context.SaveChanges();

                // Vider le panier après la commande
                var contenuPanier = context.CONTENIR.Where(c => c.id_panier == idPanier).ToList();
                context.CONTENIR.RemoveRange(contenuPanier);
                context.SaveChanges();

                // Mettre à jour la session
                Session["SommeQuantitePanier"] = 0;

                // Rediriger vers la page de confirmation de commande
                return RedirectToAction("ConfirmationCommande", new { idCommande });
            }
        }

        public decimal ConvertirEnEuro(decimal montantAriary)
        {
            // Taux de conversion fictif. Par exemple, 1 euro = 5000 Ariary
            decimal tauxConversion = 5000;
            decimal totalEuro=montantAriary / tauxConversion;
            return totalEuro;
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