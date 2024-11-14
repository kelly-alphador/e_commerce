using e_commerce.configuration;
using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace e_commerce.Controllers
{
    public class PaymentsController : Controller
    {
        // Action pour afficher la page avec le bouton PayPal
        public ActionResult PayWithPayPal()
        {
            return View();
        }

        // Action qui sera appelée lors du clic sur le bouton pour créer un paiement
        // Méthode pour convertir le montant en Ariary en Euro

        // Action qui gère la création du paiement avec PayPal
        [HttpPost]
        public ActionResult CreatePayment(decimal montantAriary)
        {
            decimal montantTotal = (montantAriary + 0.35m) / (1 - 0.029m);
            // Conversion du montant en chaîne formatée avec deux décimales
            string totalEnEuro = montantTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

            // Configuration de PayPal
            var apiContext = PayPalConfiguration.GetAPIContext();

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
        {
            new Transaction
            {
                amount = new Amount
                {
                    currency = "EUR",
                    total = totalEnEuro // Utilise le montant formaté
                },
                description = "Achat de produit X",
                 payee = new Payee
                 {
                    email = "andriamampindrykelly@gmail.com" // Remplacez par l'e-mail du compte PayPal du vendeur
                }
            }
        },
                redirect_urls = new RedirectUrls
                {
                    return_url = "https://localhost:44331/Payments/PaymentSuccess",
                    cancel_url = "https://localhost:44331/Payments/PaymentCancel"
                }
            };

            try
            {
                // Création du paiement avec PayPal
                var createdPayment = payment.Create(apiContext);

                // Récupérer l'URL d'approbation
                var approvalUrl = createdPayment.links
                    .FirstOrDefault(x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))
                    .href;

                return Redirect(approvalUrl);
            }
            catch (PaymentsException ex)
            {
                // Gérer l'exception et afficher un message d'erreur
                ViewBag.ErrorMessage = "Erreur de création du paiement PayPal : " + ex.Message;
                return View("Error"); // Vue pour afficher les erreurs
            }
        }


        // Action appelée lorsque le paiement est validé
        public ActionResult PaymentSuccess(string paymentId, string token, string PayerID)
        {
            var apiContext = PayPalConfiguration.GetAPIContext();
            var payment = new Payment() { id = paymentId };
            var executedPayment = payment.Execute(apiContext, new PaymentExecution() { payer_id = PayerID });

            if (executedPayment.state.ToLower() != "approved")
            {
                return View("FailureView");
            }

            // Ajouter ici la logique de mise à jour de la commande et d'envoi de facture

            ViewBag.PaymentId = executedPayment.id;
            ViewBag.PayerEmail = executedPayment.payer.payer_info.email;
            ViewBag.TransactionId = executedPayment.transactions[0].related_resources[0].sale.id;
            ViewBag.Amount = executedPayment.transactions[0].amount.total;
            ViewBag.Currency = executedPayment.transactions[0].amount.currency;

            return View("PaymentSuccess");
        }

        // Action appelée si l'utilisateur annule le paiement
        public ActionResult PaymentCancel()
        {
            return View("CancelView");
        }
    }
}
