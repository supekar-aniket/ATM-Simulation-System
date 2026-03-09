using ATM_Simulation_System.Areas.Identity.Data;
using ATM_Simulation_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Transactions;

namespace ATM_Simulation_System.Controllers
{
    public class TransactionController : Controller
    {
        private readonly UserManager<ATM_Simulation_SystemUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TransactionController(
            UserManager<ATM_Simulation_SystemUser> userManager,
            ApplicationDbContext dbContext)
        {
            _context = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Deposit(int accountId)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts.
                FirstOrDefault(x => x.AccountId == accountId && x.UserId==userId);

            if(account == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return View();
            }

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(int accountId, int amount)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts.
                FirstOrDefault(x =>x.AccountId==accountId && x.UserId == userId);

            if (account == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return View();
            }

            if (amount < 100)
            {
                ModelState.AddModelError("", "Minimum deposit amount is ₹100.");
            }

            if (amount > 50000)
            {
                ModelState.AddModelError("", "Maximum deposit per transaction is ₹50,000.");
            }

            if (amount % 100 != 0)
            {
                ModelState.AddModelError("", "Amount must be multiple of ₹100.");
            }

            if (!ModelState.IsValid)
            {
                return View(account);
            } 

            account.Balance += amount;

            var transaction = new Models.Transaction()
            {
                AccountId = accountId,
                Amount = amount,
                TransactionType = "Deposit",
                BalanceAfterTransaction = account.Balance,
                TransactionDate = DateTime.Now,
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("AccountInfo", "Dashboard", new { accountId = accountId });
        }

        [HttpGet]
        public IActionResult Withdraw(int accountId)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts.
                FirstOrDefault(x => x.AccountId == accountId && x.UserId == userId);

            if(account == null)
            {
                ModelState.AddModelError("", "Account not found.");
            }

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int accountId,int amount)
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts.
                FirstOrDefault(x => x.AccountId == accountId && x.UserId == userId);

            if (account == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return View();
            }

            if (amount < 100)
            {
                ModelState.AddModelError("", "Minimum withdraw amount is ₹100.");
            }

            if (amount > 50000)
            {
                ModelState.AddModelError("", "Maximum withdraw per transaction is ₹50,000.");
            }

            if (amount % 100 != 0)
            {
                ModelState.AddModelError("", "Amount must be multiple of ₹100.");
            }

            account.Balance -= amount;

            var transaction = new Models.Transaction() 
            {
                AccountId = accountId,
                Amount = amount,
                BalanceAfterTransaction = account.Balance,
                TransactionType = "Withdraw",
                TransactionDate = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("AccountInfo", "Dashboard", new { accountId = accountId });
        }


    }
}
