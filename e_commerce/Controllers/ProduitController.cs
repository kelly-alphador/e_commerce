using e_commerce.Data;
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
        public ActionResult ListesProduits()
        {
            List<PRODUIT> produits=context.PRODUIT.ToList();
            return View(produits);
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
                return RedirectToAction("ListesProduits");
            }

            return View(produit);
        }
    }
}