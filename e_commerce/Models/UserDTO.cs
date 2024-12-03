using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class UserDTO
    {
        public string id_user { get; internal set; }
        public string nom { get; internal set; }
        public string telephone { get; internal set; }
        public string adresse { get; internal set; }
        public string DateInscriptionFormatted { get; internal set; }
    }
}