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
                List<USERS> users = context.USERS.ToList();
                return View(users);
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