using e_commerce.Data;
using e_commerce.Filters;
using e_commerce.Logic;
using e_commerce.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_commerce.Controllers
{
   
    public class ProduitController : Controller
    {
        private E_COMMERCEEntities context = new E_COMMERCEEntities();
        // GET: Produit
        [AdminAuthorize]
        public ActionResult ListesProduits()
        {
            List<PRODUIT> produits = context.PRODUIT.ToList();
            return View(produits);
        }
        public ActionResult DetailleProduit(string id)
        {
            var vm = new ProduitAvecDetail();
            var rechercheprod = context.PRODUIT.Where(p => p.id_prod == id).FirstOrDefault();
            if (rechercheprod != null)
            {
                vm.id_prod = id;
                vm.nom = rechercheprod.nom;
                vm.Description = rechercheprod.description;
                vm.ImageUrl = rechercheprod.ImageUrl;
                vm.prix = rechercheprod.prix;
                vm.qte = rechercheprod.qte;
            }
            return View(vm);
        }
        // GET: Produit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Produit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PRODUIT produit, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    ImageFile.SaveAs(path);
                    produit.ImageUrl = "~/Content/Images/" + fileName; // Sauvegarde le chemin dans la base de données
                }

                context.PRODUIT.Add(produit);
                context.SaveChanges();

                // Ajouter un message dans TempData
                TempData["SuccessMessage"] = "Le produit a été ajouté avec succès.";

                return RedirectToAction("ListesProduits"); // Redirection après ajout
            }

            return View(produit);
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            PRODUIT produit = context.PRODUIT.Single(p=>p.id_prod==id);
            var produitEdit = new ProduitEdite
            {
                id_prod = produit.id_prod,
                nom=produit.nom,
                description=produit.description,
                prix=produit.prix,
                qte=produit.qte,
                ImageUrl=produit.ImageUrl,
                id_categorie=produit.id_categorie,
            };
            return View(produitEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProduitEdite produitEdit)
        {
            if (ModelState.IsValid)
            {
                var produit = context.PRODUIT.Single(p => p.id_prod == produitEdit.id_prod);

                produit.nom = produitEdit.nom;
                produit.description = produitEdit.description;
                produit.prix = produitEdit.prix;
                produit.qte = produitEdit.qte;
                produit.ImageUrl = produitEdit.ImageUrl;
                produit.id_categorie = produitEdit.id_categorie;

                context.SaveChanges();

                // Ajouter un message de succès dans TempData
                TempData["SuccessMessageEdite"] = "Le produit a été sauvegardé avec succès.";

                return RedirectToAction("ListesProduits");
            }

            return View(produitEdit);
        }

        public JsonResult Delete(string id)
        {
            using(var context=new E_COMMERCEEntities())
            {
                PRODUIT produit = context.PRODUIT.FirstOrDefault(p => p.id_prod == id);
                context.PRODUIT.Remove(produit);
                context.SaveChanges();
                return Json(new { suppression = "OK" });
            }
     
        }
        [Authorize]
        [HttpPost]
        public ActionResult SaveCommentContenir(int qte,string idprod)
        {
           
            CONTENIR newContenir = new CONTENIR();
            var pnmger = new PannierManager();
            var idUser = User.Identity.GetUserId();
            newContenir.id_panier = pnmger.RecupererIdPanier(idUser);
            newContenir.id_prod = idprod;
            

            using (var context = new E_COMMERCEEntities())
            {
                var produit = context.PRODUIT.Find(idprod);
                if(produit==null)
                {
                    return Json(new { success = false, message = "Produit introuvable" });
                }
                else
                {
                    if(produit.qte<qte)
                    {
                        // Si la quantité demandée est supérieure à celle disponible, renvoyer un message d'erreur
                        return Json(new { success = false, message = "Quantité produit insuffisante" });
                    }
                }
                newContenir.qte = qte;
                // Ajouter le produit au panier
                context.CONTENIR.Add(newContenir);
                context.SaveChanges();

                // Calculer la nouvelle somme des quantités dans le panier
                var SmMger = new SumQteInPannier();
                int sommeQuantite = SmMger.somme(newContenir.id_panier);

                // Renvoyer un résultat JSON avec succès et la somme des quantités
                return Json(new { success = true, sommeQuantites = sommeQuantite });
            }


        }
    }
}