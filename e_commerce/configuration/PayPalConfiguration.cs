using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace e_commerce.configuration
{
    public class PayPalConfiguration
    {
        public static APIContext GetAPIContext()
        {
            var clientId = System.Configuration.ConfigurationManager.AppSettings["paypal:ClientId"];
            var clientSecret = System.Configuration.ConfigurationManager.AppSettings["paypal:ClientSecret"];

            // Vérifiez si les valeurs sont nulles ou vides pour diagnostiquer plus facilement l'erreur
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("PayPal clientId ou clientSecret manquant dans le fichier web.config.");
            }

            // Obtenir un accessToken via le OAuthTokenCredential de PayPal
            string accessToken = new OAuthTokenCredential(clientId, clientSecret).GetAccessToken();

            // Retourner un nouveau contexte API PayPal
            return new APIContext(accessToken);
        }

    }
}