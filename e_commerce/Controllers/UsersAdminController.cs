using e_commerce.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_commerce.Controllers
{
    public class UsersAdminController : Controller
    {
        // GET: ListeUsersPourAdmin
        public ActionResult ListeUsers()
        {
            using (var context = new E_COMMERCEEntities())
            {
                // Récupérer tous les utilisateurs
                List<USERS> users = context.USERS.ToList();

                // Créer une liste de UserDTO avec les dates formatées
                List<Models.UserDTO> usersDTO = users.Select(u => new Models.UserDTO
                {
                    id_user = u.id_user,
                    nom = u.nom,
                    telephone = u.telephone,
                    adresse = u.adresse,
                    // Formater la date ici
                    DateInscriptionFormatted = u.date_inscription.ToString("dd/MM/yyyy")
                }).ToList();

                return View(usersDTO); // Passer la liste à la vue
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            using(var context = new E_COMMERCEEntities())
            {
                USERS user = context.USERS.FirstOrDefault(u=>u.id_user==id);
                context.USERS.Remove(user);
                context.SaveChanges();
                return Json(new { suppression = "OK" });
            }
            
        }
    }
}