using ATM_Simulation_System.Areas.Identity.Data;
using ATM_Simulation_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;

namespace ATM_Simulation_System.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ATM_Simulation_SystemUser> _userManager;
        private static readonly Random random = new Random();

        public CardController(ApplicationDbContext dbContext,UserManager<ATM_Simulation_SystemUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateCard(int accountId)
        {
            var account = _dbContext.Accounts.FirstOrDefault(x => x.AccountId == accountId);

            if (account == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return RedirectToAction("Index", "Dashboard");
            }

            var card = new Card
            {
                AccountId = accountId,
                CreatedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(5)
            };

            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(Card card, string pin, string confirmPin)
        {
            ModelState.Remove("CardNumber");
            ModelState.Remove("PinHash");
            ModelState.Remove("Account");

            if (pin != confirmPin)
            {
                ModelState.AddModelError("", "PIN and Confirm PIN do not match.");
                return View(card);
            }

            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4)
            {
                ModelState.AddModelError("", "PIN must be exactly 4 digits.");
                return View(card);
            }

            if (!pin.All(char.IsDigit))
            {
                ModelState.AddModelError("", "PIN must contain only numbers.");
                return View(card);
            }

            // Check if card already exists
            var existingCard = _dbContext.Cards.Any(x => x.AccountId == card.AccountId);

            if (existingCard)
            {
                ModelState.AddModelError("", "Card already generated for this account.");
                return View(card);
            }

            var account = _dbContext.Accounts.FirstOrDefault(x => x.AccountId == card.AccountId);

            if (account == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return View(card);
            }

            card.CardNumber = GenerateCardNumber(account.AccountNumber);

            var hasher = new PasswordHasher<Card>();
            card.PinHash = hasher.HashPassword(card, pin);

            card.CreatedDate = DateTime.Now;
            card.ExpiryDate = DateTime.Now.AddYears(5);
            card.IsActive = true;

            _dbContext.Cards.Add(card);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }


        private string GenerateCardNumber(string accountNumber)
        {
            string firstFour = accountNumber.Substring(0, 4);
            string remaining = "";

            for (int i = 0; i < 12; i++)
            {
                remaining += random.Next(0, 10).ToString();
            }

            return firstFour + remaining;
        }
    }
}
