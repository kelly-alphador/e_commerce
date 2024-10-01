using e_commerce.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_commerce.Logic
{
    public class SumQteInPannier
    {
        public int somme(int idpanier)
        {
            using(var context=new E_COMMERCEEntities())
            {
               // int somme = context.CONTENIR.Where(c => c.id_panier == idpanier).Sum(c => (int?)c.qte ?? 0);
                int somme = context.CONTENIR
    .Where(c => c.id_panier == idpanier)
    .Select(c => c.qte)
    .DefaultIfEmpty(0)
    .Sum();

                return somme;
            }
       
        }
    }
}
