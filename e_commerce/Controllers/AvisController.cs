using e_commerce.Data;
using e_commerce.Logic;
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
        public ActionResult ListeaAvisAdmin()
        {
            using(var context=new E_COMMERCEEntities())
            {
                var avisList = (from a in context.AVIS
                                join u in context.USERS on a.id_user equals u.id_user
                                join p in context.PRODUIT on a.id_prod equals p.id_prod
                                select new AvisViewModelNomProdUser
                                {
                                    idAvis=a.id_avis,
                                    commentaire = a.commentaire,
                                    Note = a.note,
                                    DateAvis = a.date_avis,
                                    Nomuser = u.nom,
                                    NomProd = p.nom,
                                }).ToList();
                return View(avisList);
            }
        }
        public ActionResult ListesAvis(string idProd)
        {
            using (var context = new E_COMMERCEEntities())
            {
                //select des avis (id_produit,commentaire,Note,NomPersonne
                var avis = context.AVIS
                    .Where(a => a.id_prod == idProd)
                    .Select(a => new AvisViewModel
                    {
                        id_produit = a.id_prod,
                        Commentaire = a.commentaire,
                        Note = a.note,
                        NomPersonne = context.USERS
                            .Where(u => u.id_user == a.id_user)
                            .Select(u => u.nom)
                            .FirstOrDefault()
                    })
                    .ToList();

                // Retourner l'ID du produit en plus de la liste d'avis
                ViewBag.IdProd = idProd;

                return View(avis);
            }


            }
      
        [Authorize]
        public ActionResult LaisserAvis(string idprod)
        {
            var vm = new LaisserAvis();
            using (var contexte = new E_COMMERCEEntities())
            {
                //verifie si l'idproduit existe
                var produitEntity = contexte.PRODUIT.FirstOrDefault(p => p.id_prod == idprod);
                if (produitEntity != null)
                {
                    vm.idprod = produitEntity.id_prod;
                    vm.nomprod = produitEntity.nom;
                }
            }

            return View(vm);
        }
        public ActionResult SaveComment(string commentaire, string note, string idprod)
        {
            // System.Diagnostics.Debug.WriteLine("Début de SaveComment");

            AVIS newAvis = new AVIS();
            //pour recuperer la date d'aujourd'hui
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
            // Rediriger vers l'action ListesAvis après avoir enregistré le commentaire
            return RedirectToAction("ListesAvis", "Avis", new { idProd = idprod });
        }
        public JsonResult Delete(int id)
        {
            using(var context=new E_COMMERCEEntities())
            {
                var AvisEntity = context.AVIS.FirstOrDefault(a => a.id_avis == id);
                context.AVIS.Remove(AvisEntity);
                context.SaveChanges();
                return Json(new { suppression = "OK" });
            }
         }
    }
}