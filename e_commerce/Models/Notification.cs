using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false; // Valeur par défaut
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Valeur par défaut
        public string NotificationType { get; set; }
    }
}