using e_commerce.configuration;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        [HttpPost]
        public ActionResult CreatePayment()
        {
            var apiContext = PayPalConfiguration.GetAPIContext();

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount { currency = "USD", total = "10.00" },
                        description = "Achat de produit X"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = "http://votresite.com/Payments/PaymentSuccess",
                    cancel_url = "http://votresite.com/Payments/PaymentCancel"
                }
            };

            var createdPayment = payment.Create(apiContext);
            var approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase)).href;

            return Redirect(approvalUrl);
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

            // Mettre à jour la commande et marquer comme payée dans la base de données
            // Envoyer une facture ou confirmation à l'utilisateur

            return View("SuccessView");
        }

        // Action appelée si l'utilisateur annule le paiement
        public ActionResult PaymentCancel()
        {
            return View("CancelView");
        }
    }
}
