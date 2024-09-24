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
        public ActionResult Index(string searchTerm)
        {
            /* using (var context = new E_COMMERCEEntities())
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
             }*/
            using (var context = new E_COMMERCEEntities())
            {
                // Récupérer tous les produits
                var produitsQuery = context.PRODUIT.AsQueryable();

                // Si un terme de recherche est fourni, filtrer les produits
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    produitsQuery = produitsQuery.Where(p => p.nom.Contains(searchTerm));
                }

                // Créer une liste de ViewModelNomImage avec les produits filtrés
                List<ViewModelNomImage> produits = produitsQuery.Select(p => new ViewModelNomImage
                {
                    id = p.id_prod,
                    nom = p.nom,
                    imageUrl = p.ImageUrl,
                    nombreAvis = p.AVIS.Count,
                }).ToList();

                // Passer la liste de produits à la vue
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