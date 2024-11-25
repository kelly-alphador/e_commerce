using e_commerce.Data;
using e_commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using e_commerce.Logic;

namespace e_commerce.Controllers
{
    public class ContenirController : Controller
    {
        
        // GET: Contenir
        [Authorize]
        public ActionResult ListeContenir()
        {
            var idUser = User.Identity.GetUserId();
            using (var context = new E_COMMERCEEntities())
            {
                // Récupérer les données du panier
                var ListeContenir = (from c in context.CONTENIR
                                     join p in context.PRODUIT on c.id_prod equals p.id_prod
                                     join pa in context.PANIER on c.id_panier equals pa.id_panier
                                     select new ContenirViewModel
                                     {
                                         id_user = pa.id_user,
                                         id_panier = c.id_panier,
                                         id_prod = c.id_prod,
                                         nom_prod = p.nom,
                                         qte = c.qte,
                                         prix_prod = p.prix,
                                         prix = c.qte * p.prix,
                                     }).Where(c => c.id_user == idUser).ToList();

                // Total en Ariary
                int TotalAriary = ListeContenir.Sum(i => i.prix);
                ViewBag.TotalAriary = TotalAriary;

                // Conversion en euros
                decimal TotalEuro = ConvertirEnEuro(TotalAriary);
                ViewBag.TotalEuro = TotalEuro;

                // Frais de transfert
                decimal FraisTransfert = 0.35m + (TotalEuro * 0.029m);
                ViewBag.FraisTransfert = FraisTransfert;

                // Somme totale à payer (euros + frais)
                decimal MontantTotal = TotalEuro + FraisTransfert;
                ViewBag.MontantTotal = MontantTotal;

                return View(ListeContenir);
            }
        }

        private decimal ConvertirEnEuro(decimal montantAriary)
        {
            // Taux fictif : 1 euro = 5000 Ariary
            decimal tauxConversion = 5000;
            return montantAriary / tauxConversion;
        }


        [HttpPost]
        public JsonResult Delete(string id, int idPanier)
        {
            using (var context = new E_COMMERCEEntities())
            {
               
                CONTENIR contenir = context.CONTENIR.FirstOrDefault(c => c.id_prod == id && c.id_panier == idPanier);

                if (contenir != null)
                {
                    context.CONTENIR.Remove(contenir);
                    context.SaveChanges();

                    // Calculer la nouvelle quantité totale et le nouveau prix total
                    var SumManager = new SumQteInPannier();
                    var totalItems = SumManager.somme(idPanier);
                    var totalPrice = context.CONTENIR
                        .Where(c => c.id_panier == idPanier)
                        .Sum(c => c.qte * c.PRODUIT.prix);
                    var TotalEuro = ConvertirEnEuro(totalPrice);
                    var FraisTransfert = 0.35m + (TotalEuro * 0.029m);
                    var MontantTotal = TotalEuro + FraisTransfert;

                    return Json(new { success = true, totalItems, totalPrice, TotalEuro, FraisTransfert,MontantTotal });
                }

                return Json(new { success = false, message = "Article non trouvé." });
            }
        }
        [HttpPost]
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