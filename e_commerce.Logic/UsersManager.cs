using e_commerce.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_commerce.Logic
{
    public class UsersManager
    {
        public string NomUserbyUserId(string userId)
        {
            using (var context = new E_COMMERCEEntities())
            {
                var userEntity = context.USERS.FirstOrDefault(u => u.id_user == userId);
                if(userEntity==null)
                {
                    return "Anonym";
                }
                return userEntity.nom;
            }
        }
        public void InsertUsers(string userId,string nom,string tel,string adresse)
        {
            using(var context=new E_COMMERCEEntities())//pour liberer les resource qui va appelee le Dispose() du context
            {
                //pour verifier si l'iduser existe deja dans la table
                var userEntity = context.USERS.FirstOrDefault(u => u.id_user == userId);
                if(userEntity==null)
                {
                    try
                    {
                        var u = new USERS();
                        u.id_user = userId;
                        u.nom = nom;
                        u.telephone = tel;
                        u.adresse = adresse;
                        u.date_inscription = DateTime.Today;
                        context.USERS.Add(u);
                        context.SaveChanges();
                    }
                    catch(DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                // Affichez les messages d'erreur
                                Console.WriteLine($"Propriété: {validationError.PropertyName}, Erreur: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }
            }
        }
    }
}
