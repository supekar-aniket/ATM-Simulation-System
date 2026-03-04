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
        public async Task<IActionResult> CreateAccount(string accountType)
        {
            var userId = _userManager.GetUserId(User);

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
            var guid = Guid.NewGuid().ToString("N"); // remove hyphens

            string numbers = new string(guid.Where(char.IsDigit).ToArray());

            if (numbers.Length < 16)
                numbers = numbers.PadRight(16, '0');

            return numbers.Substring(0, 16);
        }
    }
}
