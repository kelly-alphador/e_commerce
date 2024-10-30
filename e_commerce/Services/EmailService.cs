using System;
using System.Net;
using System.Net.Mail;

namespace e_commerce.Services
{
    public class EmailService_by
    {
        // Définissez le seuil de stock minimum
        private const int StockThreshold = 3; // Ajustez selon vos besoins

        public void SendStockAlert(string productName, int availableQuantity)
        {
            // Vérifiez si la quantité disponible est inférieure au seuil
            if (availableQuantity < StockThreshold)
            {
                var fromAddress = new MailAddress("renotolivia@gmail.com", "Nom de votre application");
                var toAddress = new MailAddress("kellyalphador@example.com", "Administrateur");
                const string subject = "Alerte de rupture de stock";
                string body = $"Le produit '{productName}' est en rupture de stock. Quantité disponible: {availableQuantity}.";

                try
                {
                    using (var smtp = new SmtpClient())
                    {
                        smtp.EnableSsl = true;
                        smtp.Host = "smtp.gmail.com"; // SMTP de Gmail
                        smtp.Port = 587; // Port pour Gmail
                        smtp.Credentials = new NetworkCredential(fromAddress.Address, "andriamampindrykelly2004"); // Remplacez par votre mot de passe

                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = subject,
                            Body = body
                        })
                        {
                            smtp.Send(message);
                        }
                    }
                }
                catch (SmtpException smtpEx)
                {
                    // Gestion des erreurs SMTP
                    Console.WriteLine($"Erreur lors de l'envoi de l'e-mail: {smtpEx.Message}");
                    // Vous pouvez également enregistrer l'erreur dans un fichier ou une base de données
                }
                catch (Exception ex)
                {
                    // Gestion des autres erreurs
                    Console.WriteLine($"Erreur inattendue: {ex.Message}");
                }
            }
        }
    }
}
