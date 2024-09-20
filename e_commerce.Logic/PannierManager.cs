using e_commerce.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_commerce.Logic
{
    public class PannierManager
    {
        public void InsertionPannier(string id_user)
        {
            using(var context=new E_COMMERCEEntities())
            {
                try
                {
                    var pannier = new PANIER();
                    pannier.id_user = id_user;
                    context.PANIER.Add(pannier);
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
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
