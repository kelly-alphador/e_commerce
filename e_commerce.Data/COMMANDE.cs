//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace e_commerce.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class COMMANDE
    {
        public int id_com { get; set; }
        public Nullable<System.DateTime> date_commande { get; set; }
        public string id_users { get; set; }
    
        public virtual USERS USERS { get; set; }
    }
}
