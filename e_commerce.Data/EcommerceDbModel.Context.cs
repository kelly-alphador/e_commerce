﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class E_COMMERCEEntities : DbContext
    {
        public E_COMMERCEEntities()
            : base("name=E_COMMERCEEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AVIS> AVIS { get; set; }
        public virtual DbSet<CATEGORIE> CATEGORIE { get; set; }
        public virtual DbSet<COMMANDE> COMMANDE { get; set; }
        public virtual DbSet<CONTENIR> CONTENIR { get; set; }
        public virtual DbSet<DETAIL_COMMANDE> DETAIL_COMMANDE { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<PANIER> PANIER { get; set; }
        public virtual DbSet<PRODUIT> PRODUIT { get; set; }
        public virtual DbSet<USERS> USERS { get; set; }
    }
}
