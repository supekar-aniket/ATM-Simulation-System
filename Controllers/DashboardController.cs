using ATM_Simulation_System.Areas.Identity.Data;
using ATM_Simulation_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ATM_Simulation_System.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ATM_Simulation_SystemUser> _userManager;

        public DashboardController(ApplicationDbContext dbContext, UserManager<ATM_Simulation_SystemUser> userManager)
        {
            _context = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var accounts = _context.Accounts
                .Include(a => a.Cards)   // load card
                .Where(a => a.UserId == userId)
                .ToList();

            ViewBag.HasAccount = accounts.Any();

            return View(accounts);
        }

        [HttpGet]
        public IActionResult VerifyPin(int accountId)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts
                .Include(a => a.Cards)
                .FirstOrDefault(a => a.AccountId == accountId && a.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("Index");
            }

            if (!account.Cards.Any())
            {
                TempData["Error"] = "Please generate card first.";
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = accountId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyPin(string pin, int accountId)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts
                .Include(x => x.Cards)
                .FirstOrDefault(x => x.AccountId == accountId && x.UserId == userId);

            var card = account.Cards.FirstOrDefault();

            var hasher = new PasswordHasher<Card>();
            var result = hasher.VerifyHashedPassword(card, card.PinHash, pin);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid PIN.");
                ViewBag.AccountId = accountId;
                return View();
            }

            return RedirectToAction("AccountInfo", new { accountId });
        }

        [HttpGet]
        public IActionResult AccountInfo(int accountId)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts
                .Include(x => x.User)
                .Include(x => x.Cards)
                .FirstOrDefault(a => a.AccountId == accountId && a.UserId == userId);

            if (account == null)        
            {
                return RedirectToAction("Index");
            }

            return View(account);
        }


    }
}
