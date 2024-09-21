using e_commerce.Data;
using e_commerce.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_commerce.Controllers
{
    public class AvisController : Controller
    {
        // GET: Avis
        public ActionResult LaisserAvis(string idprod)
        {
            var vm = new LaisserAvis();
            using(var contexte=new E_COMMERCEEntities())
            {
                var produitEntity = contexte.PRODUIT.FirstOrDefault(p => p.id_prod == idprod);
                if(produitEntity!=null)
                {
                    vm.idprod = produitEntity.id_prod;
                    vm.nomprod = produitEntity.nom;
                }
            }
            return View(vm);
        }
        public ActionResult SaveComment(string commentaire, string note,string idprod)
        {
            // System.Diagnostics.Debug.WriteLine("Début de SaveComment");

            AVIS newAvis = new AVIS();
            newAvis.date_avis = DateTime.Now;
            newAvis.commentaire = commentaire;
            newAvis.id_user = User.Identity.GetUserId();
            double bNote = 0;

            if (!double.TryParse(note, NumberStyles.Any, CultureInfo.InvariantCulture, out bNote))
            {
                throw new Exception("IMpossible de parser la note" + note);
            }

            newAvis.note = bNote;

            using (var context = new E_COMMERCEEntities())
            {
                var produitEntity = context.PRODUIT.FirstOrDefault(p => p.id_prod == idprod);
                if (produitEntity == null)
                {
                    return RedirectToAction("Acceuil", "Home");
                }
                newAvis.id_prod = produitEntity.id_prod;
                context.AVIS.Add(newAvis);
                context.SaveChanges();
            }
            return View();
        }
    }
}