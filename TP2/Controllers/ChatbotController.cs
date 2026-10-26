using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TP2.Models.Repositories;
using TP2.ViewModels;

namespace TP2.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatbotController(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            UserManager<IdentityUser> userManager)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return Json(new ChatResponse { Reply = "Bonjour ! Comment puis-je vous aider aujourd'hui ?" });
            }

            string msg = request.Message.Trim().ToLower();
            var response = new ChatResponse();
            bool isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");
            bool isAuthenticated = User.Identity.IsAuthenticated;

            // -------------------------------------------------------------
            // 1. INTENTIONS SPÉCIFIQUES ADMIN & MANAGER
            // -------------------------------------------------------------
            if (isAdminOrManager)
            {
                // Suivi de Stock & Ruptures
                if (msg.Contains("stock") || msg.Contains("rupture"))
                {
                    var lowStockProducts = _productRepository.GetAllProducts()
                        .Where(p => p.QteStock <= 3)
                        .ToList();

                    if (lowStockProducts.Any())
                    {
                        response.Reply = "⚠️ **Alerte Stock Faible / Rupture :**<br/>" +
                            string.Join("<br/>", lowStockProducts.Select(p => $"• **{p.Name}** : {p.QteStock} unité(s) restante(s)"));
                    }
                    else
                    {
                        response.Reply = "✅ Tous les produits ont un niveau de stock suffisant.";
                    }
                    response.Suggestions = new List<string> { "Chiffre d'affaires", "Dernières commandes" };
                    return Json(response);
                }

                // Statistiques Ventes & CA
                if (msg.Contains("vente") || msg.Contains("chiffre") || msg.Contains("ca") || msg.Contains("revenu"))
                {
                    var orders = _orderRepository.GetAllOrders().ToList();
                    var totalRevenue = orders.Sum(o => o.TotalAmount);
                    var totalOrders = orders.Count;

                    response.Reply = $"📊 **Statistiques Globales :**<br/>• **Commandes totales :** {totalOrders}<br/>• **Chiffre d'affaires total :** {totalRevenue:N2} DT";
                    response.Suggestions = new List<string> { "Produits en rupture", "Dernières commandes" };
                    return Json(response);
                }

                // Dernières commandes pour Admin
                if (msg.Contains("commandes") || msg.Contains("ordres"))
                {
                    var recentOrders = _orderRepository.GetAllOrders()
                        .OrderByDescending(o => o.OrderDate)
                        .Take(3)
                        .ToList();

                    if (recentOrders.Any())
                    {
                        response.Reply = "📋 **3 Dernières commandes :**<br/>" +
                            string.Join("<br/>", recentOrders.Select(o => $"• N°**#{o.Id}** par {o.CustomerName} - {o.TotalAmount:N2} DT"));
                    }
                    else
                    {
                        response.Reply = "Aucune commande trouvée dans le système.";
                    }
                    response.Suggestions = new List<string> { "Chiffre d'affaires", "Produits en rupture" };
                    return Json(response);
                }
            }

            // -------------------------------------------------------------
            // 2. INTENTIONS CLIENT & VISITEUR
            // -------------------------------------------------------------
            // Salutations
            if (msg.Contains("bonjour") || msg.Contains("salut") || msg.Contains("hello") || msg.Contains("coucou"))
            {
                string name = isAuthenticated ? User.Identity.Name : "visiteur";
                response.Reply = $"Bonjour **{name}** ! 👋 Je suis l'assistant virtuel E-Shop. Comment puis-je vous aider ?";
                response.Suggestions = isAuthenticated ?
                    new List<string> { "Où est ma commande ?", "Frais de livraison", "Voir le catalogue" } :
                    new List<string> { "Voir le catalogue", "Frais de livraison" };
                return Json(response);
            }

            // Suivi de commande personnelle
            if (msg.Contains("commande") || msg.Contains("suivi") || msg.Contains("colis"))
            {
                if (!isAuthenticated)
                {
                    response.Reply = "🔒 Veuillez vous connecter pour consulter le statut de vos commandes.";
                    response.Suggestions = new List<string> { "Voir le catalogue", "Frais de livraison" };
                    return Json(response);
                }

                var user = await _userManager.GetUserAsync(User);
                var userOrders = _orderRepository.GetOrdersByUserId(user?.Id);
                var lastOrder = userOrders?.OrderByDescending(o => o.OrderDate).FirstOrDefault();

                if (lastOrder != null)
                {
                    response.Reply = $"📦 **Votre dernière commande (N°#{lastOrder.Id})** :<br/>• Date : {lastOrder.OrderDate:dd/MM/yyyy HH:mm}<br/>• Total : {lastOrder.TotalAmount:N2} DT<br/>• Adresse : {lastOrder.Address}";
                }
                else
                {
                    response.Reply = "Vous n'avez encore passé aucune commande sur notre boutique.";
                }
                response.Suggestions = new List<string> { "Voir le catalogue", "Frais de livraison" };
                return Json(response);
            }

            // Informations de livraison
            if (msg.Contains("livraison") || msg.Contains("frais") || msg.Contains("expédition"))
            {
                response.Reply = "🚚 **Informations de livraison :**<br/>• Expédition partout en Tunisie sous 24h à 48h.<br/>• Frais de livraison : **7 DT** (Gratuit à partir de 150 DT d'achats).";
                response.Suggestions = new List<string> { "Moyens de paiement", "Voir le catalogue" };
                return Json(response);
            }

            // Moyens de paiement
            if (msg.Contains("paiement") || msg.Contains("payer") || msg.Contains("carte"))
            {
                response.Reply = "💳 **Moyens de paiement acceptés :**<br/>• Paiement en espèces à la livraison.<br/>• Paiement sécurisé en ligne.";
                response.Suggestions = new List<string> { "Frais de livraison", "Voir le catalogue" };
                return Json(response);
            }

            // Produits et catalogue
            if (msg.Contains("produit") || msg.Contains("catalogue") || msg.Contains("article"))
            {
                var products = _productRepository.GetAllProducts().ToList();
                var popular = products.Take(2).ToList();
                response.Reply = $"🛍️ Nous disposons de **{products.Count} articles** en catalogue.<br/>À la une :<br/>" +
                    string.Join("<br/>", popular.Select(p => $"• **{p.Name}** ({p.Price:N2} DT)"));
                response.Suggestions = new List<string> { "Frais de livraison", "Où est ma commande ?" };
                return Json(response);
            }

            // Réponse par défaut
            response.Reply = "Je n'ai pas bien compris votre question. Voici quelques sujets sur lesquels je peux vous renseigner :";
            response.Suggestions = isAdminOrManager
                ? new List<string> { "Chiffre d'affaires", "Produits en rupture", "Dernières commandes" }
                : new List<string> { "Où est ma commande ?", "Frais de livraison", "Moyens de paiement", "Voir le catalogue" };

            return Json(response);
        }
    }
}