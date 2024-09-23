using e_commerce.Data;
using e_commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_commerce.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new E_COMMERCEEntities())
            {
                // Créer une liste de ViewModelNomImage
                List<ViewModelNomImage> produits = context.PRODUIT.Select(p => new ViewModelNomImage
                {
                    id=p.id_prod,
                    nom = p.nom,
                    imageUrl = p.ImageUrl,
                    nombreAvis=p.AVIS.Count,
                }).ToList();

                // Passer la liste de ViewModel à la vue
                return View(produits);
            }
        }

      /*  public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }*/
    }
}