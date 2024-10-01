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
            using(var context=new E_COMMERCEEntities())
            {
                //pour recuperer les donnees de panier et produit et contenir
                var ListeContenir=(from c in context.CONTENIR
                                   join p in context.PRODUIT on c.id_prod equals p.id_prod
                                   join pa in context.PANIER on c.id_panier equals pa.id_panier
                                   select new ContenirViewModel
                                   {
                                       id_user=pa.id_user,
                                       id_panier=c.id_panier,
                                       id_prod=c.id_prod,
                                       nom_prod=p.nom,
                                       qte=c.qte,
                                       prix_prod=p.prix,
                                       prix=c.qte*p.prix,
                                   }
                                   ).Where(c=>c.id_user==idUser).ToList();
                int Total = ListeContenir.Sum(i => i.prix);
                ViewBag.Total = Total;
                return View(ListeContenir);
            }

            
        }
  
        [HttpPost]
        public JsonResult Delete(string id, int idPanier)
        {
            using (var context = new E_COMMERCEEntities())
            {
                // Trouver l'élément à supprimer
                /* CONTENIR contenir = context.CONTENIR
                     .FirstOrDefault(c => c.id_prod == id && c.id_panier == idPanier);

                 if (contenir != null)
                 {
                     context.CONTENIR.Remove(contenir);
                     context.SaveChanges();

                     // Calculer la nouvelle quantité totale dans le panier
                     var SumManager = new SumQteInPannier();
                     var totalItems = SumManager.somme(idPanier);

                     return Json(new { success = true, totalItems });  // Clé "success"
                 }

                 return Json(new { success = false, message = "Article non trouvé." });*/
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

                    return Json(new { success = true, totalItems, totalPrice });
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

        /*[HttpPost]
        public JsonResult ViderPanier()
        {
            var idUser = User.Identity.GetUserId();
            using(var context= new E_COMMERCEEntities())
            {
                var EntityPanier = context.PANIER.FirstOrDefault(p => p.id_user == idUser);
                if(EntityPanier!=null)
                {
                    var ContenuPanier = context.CONTENIR.Where(c => c.id_panier == EntityPanier.id_panier).ToList();
                    context.CONTENIR.RemoveRange(ContenuPanier);
                    context.SaveChanges();

                    var SmMger = new SumQteInPannier();
                    var Pmger = new PannierManager();
                    int idpanier = Pmger.RecupererIdPanier(idUser);
                    Console.WriteLine(idpanier);
                    int sommeQuantite = SmMger.somme(idpanier);
                    Console.WriteLine(sommeQuantite);
                    //TempData["SommeQuantitePanier"] = sommeQuantite;

                    // Stocker la somme dans la session

                    Session["SommeQuantitePanier"] = sommeQuantite;
                    Console.WriteLine("SommeQuantitePanier dans la session : " + Session["SommeQuantitePanier"]);
                    return Json(new { success = true, message = "Panier vidé avec succès." });
                }

                return Json(new { success = false, message = "Panier introuvable." });
            }
        }*/
    }
}