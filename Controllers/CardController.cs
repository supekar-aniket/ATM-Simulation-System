using ATM_Simulation_System.Areas.Identity.Data;
using ATM_Simulation_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;

namespace ATM_Simulation_System.Controllers
{
    [Authorize]
    public class CardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ATM_Simulation_SystemUser> _userManager;
        private static readonly Random random = new Random();

        public CardController(ApplicationDbContext context,UserManager<ATM_Simulation_SystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateCard(int accountId)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts.
                FirstOrDefault(x => x.AccountId == accountId && x.UserId == userId);

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
            var existingCard = _context.Cards.Any(x => x.AccountId == card.AccountId);

            if (existingCard)
            {
                ModelState.AddModelError("", "Card already generated for this account.");
                return View(card);
            }

            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts.
                FirstOrDefault(x => x.AccountId == card.AccountId && x.UserId == userId);

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

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult ChangePin(int accountId)
        {
            var card = _context.Cards.FirstOrDefault(x => x.AccountId == accountId);

            if(card == null)
            {
                ModelState.AddModelError("", "Card not found.");
            }

            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePin(int accountId, string oldPin, string newPin, string confirmPin)
        {
            var userId = _userManager.GetUserId(User);

            var card = _context.Cards.
                Include(x => x.Account).
                FirstOrDefault(x => x.AccountId == accountId && x.Account.UserId == userId);

            if (card == null)
            {
                ModelState.AddModelError("", "Card not found.");
                return View();
            }

            var hasher = new PasswordHasher<Card>();

            var result = hasher.VerifyHashedPassword(card, card.PinHash, oldPin);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Old PIN is incorrect.");
                return View(card);
            }

            if (newPin.Length != 4 || !newPin.All(char.IsDigit))
            {
                ModelState.AddModelError("", "PIN must be 4 digits.");
                return View(card);
            }

            if (newPin != confirmPin)
            {
                ModelState.AddModelError("", "New PIN and Confirm PIN do not match.");
                return View(card);
            }

            // Hash new PIN
            card.PinHash = hasher.HashPassword(card, newPin);

            await _context.SaveChangesAsync();

            return RedirectToAction("AccountInfo", "Dashboard");
        }

        [HttpGet]
        public IActionResult ShowCard(int accountId)
        {
            var userId = _userManager.GetUserId(User);

            var card = _context.Cards
                .Include(x => x.Account)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.AccountId == accountId && x.Account.UserId == userId);

            if (card == null)
            {
                ModelState.AddModelError("", "Card not found.");
                return RedirectToAction("Index", "Dashboard");
            }

            return View(card);
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
