using ATM_Simulation_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ATM_Simulation_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ATM_Simulation_SystemUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ATM_Simulation_SystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.TotalUsers = _userManager.Users.Count();
            ViewBag.TotalAccounts = _context.Accounts.Count();
            ViewBag.TotalCards = _context.Cards.Count();
            ViewBag.TotalTransactions = _context.Transactions.Count();

            return View();
        }
    }
}
