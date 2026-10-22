using CrustyAPI.DTOs;

namespace CrustyAPI.Services;

public class CrabbyBotService
{
    public CrabbybotResponse GetResponse(string optionId)
    {
        return optionId switch
        {
            "start" => new CrabbybotResponse
            {
                Message = "I'm Crabby Bot  🦀, your Krusty Krab assistant! What can I help you with today?",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "menu", Text = "🍔 Menu" },
                    new() { Id = "delivery", Text = "🚚 Delivery" },
                    new() { Id = "ordering", Text = "🛒 Ordering" },
                    new() { Id = "payments", Text = "💳 Payments" },
                    new() { Id = "hours", Text = "🕐 Opening Hours" },
                    new() { Id = "contact", Text = "📞 Contact Us" }
                }
            },

            "menu" => new CrabbybotResponse
            {
                Message = "Hungry already? 🍔 Let's talk about the good stuff! What would you like to know about our menu?",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "menu_view", Text = "📋 View Menu" },
                    new() { Id = "menu_popular", Text = "⭐ Popular Items" },
                    new() { Id = "start", Text = "🔙 Main Menu" }
                }
            },

            "menu_view" => new CrabbybotResponse
            {
                Message = "You can browse all our available menu items in the Menu section of the app. Go on, treat yourself! 🦞",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "menu", Text = "🔙 Back to Menu" },
                    new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },

            "menu_popular" => new CrabbybotResponse
            {
                Message = "Our  favourites include our boil bags, the best in Jhb! ⭐🦞 Which one are you craving?",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "menu", Text = "🔙 Back to Menu" },
                    new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },
            //DELIVERY
            "delivery" => new CrabbybotResponse
            {
                Message = "🦞 We deliver anywhere in Johannesburg! Delivery is charged to the customer and the price is based on the current Uber delivery rate for your location.",
                Options = new List<CrabbybotOptionResponse>
            {
                new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },

           
            //ORDER
            "ordering" => new CrabbybotResponse
            {
                Message = "Ready to place an order? 🛒 Shell yeah! What would you like to know?",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "ordering_how", Text = "🛒 How do I place an order?" },
                    new() { Id = "ordering_cancel", Text = "❌ Can I cancel an order?" },
                    new() { Id = "start", Text = "🔙 Main Menu" }
                }
            },

            "ordering_how" => new CrabbybotResponse
            {
                Message = "easy! browse the menu and pick you choice of meal. Then add to cart and proceed to checkout!",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "ordering", Text = "🔙 Back to Ordering" },
                    new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },

            "ordering_cancel" => new CrabbybotResponse
            {
                Message = "Need to cancel an order? Contact the restaurant as soon as possible and we'll do our best to help. 🦀",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "ordering", Text = "🔙 Back to Ordering" },
                    new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },
            //PAYMENTS
            "payments" => new CrabbybotResponse
            {
                Message = "Let's talk money! 💰 What would you like to know about payments?",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "payment_methods", Text = "💳 Payment Methods" },
                    new() { Id = "payment_problem", Text = "⚠️ Payment Problem" },
                    new() { Id = "start", Text = "🔙 Main Menu" }
                }
            },

            "payment_methods" => new CrabbybotResponse
            {
                Message = "We accept card, and cash only for for returning customers",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "payments", Text = "🔙 Back to Payments" },
                    new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },

            "payment_problem" => new CrabbybotResponse
            {
                Message = "Uh oh! 🦀 If your payment failed or something went wrong, don't panic. Contact the restaurant for assistance and we'll help get things sorted.",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "payments", Text = "🔙 Back to Payments" },
                    new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },
            //HOURS
            "hours" => new CrabbybotResponse
            {
                Message = "We're open from 8:00 to 19:00 every day!",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "start", Text = "🏠 Main Menu" }
                }
            },
            //CONTACT
            "contact" => new CrabbybotResponse
            {
                Message = "Need to speak to a real person? 📞🦞 Feel free to call or WhatsApp us on 081 234 1234. We're happy to help with any questions or issues you may have!",
                Options = new List<CrabbybotOptionResponse>
    {
        new() { Id = "start", Text = "🏠 Main Menu" }
    }
            },

            _ => new CrabbybotResponse
            {
                Message = "Hmm... 🦀 I don't recognise that option. Let's start over!",
                Options = new List<CrabbybotOptionResponse>
                {
                    new() { Id = "start", Text = "🔄 Start Again" }
                }
            }
        };
    }
}