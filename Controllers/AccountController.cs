using ATM_Simulation_System.Areas.Identity.Data;
using ATM_Simulation_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATM_Simulation_System.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ATM_Simulation_SystemUser> _userManager;

        public AccountController(ApplicationDbContext dbContext,UserManager<ATM_Simulation_SystemUser> userManager)
        {
            _context = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(string accountType)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(accountType))
            {
                ModelState.AddModelError("", "Please select an account type.");
                return View();
            }

            var totalAccounts = await _context.Accounts
                .CountAsync(a => a.UserId == userId);

            if (totalAccounts >= 2)
            {
                ModelState.AddModelError("", "You already have the maximum number of accounts.");
                return View();
            }

            var existingAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.AccountType == accountType);

            if (existingAccount != null)
            {
                ModelState.AddModelError("", $"You already have a {accountType} account.");
                return View();
            }

            Account account = new Account
            {
                UserId = userId,
                AccountNumber = GenerateAccountNumber(),
                AccountType = accountType,
                CreatedDate = DateTime.Now,
                Balance = accountType == "Savings" ? 1000 : 5000
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }

        private string GenerateAccountNumber()
        {
            Random random = new Random();
            string number = "";

            for (int i = 0; i < 16; i++)
            {
                number += random.Next(0, 10).ToString();
            }

            return number;
        }
    }
}
