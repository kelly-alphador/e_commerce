using e_commerce.Data;
using e_commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace e_commerce.Controllers
{
    public class ContenirController : Controller
    {
        
        // GET: Contenir
        public ActionResult ListeContenir()
        {
            var idUser = User.Identity.GetUserId();
            using(var context=new E_COMMERCEEntities())
            {
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
                CONTENIR contenir = context.CONTENIR
                    .FirstOrDefault(c => c.id_prod == id && c.id_panier == idPanier);

                if (contenir != null)
                {
                    context.CONTENIR.Remove(contenir);
                    context.SaveChanges();
                    return Json(new { suppression = "OK" });
                }

                return Json(new { suppression = "NOT_FOUND" }); // Gérer le cas où l'élément n'est pas trouvé
            }
        }
    }
}