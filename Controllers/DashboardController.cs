using ATM_Simulation_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ATM_Simulation_System.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ATM_Simulation_SystemUser> _userManager;

        public DashboardController(ApplicationDbContext dbContext,UserManager<ATM_Simulation_SystemUser> userManager)
        {
            _context = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var account = _context.Accounts.FirstOrDefault(x => x.UserId == userId);

            ViewBag.HasAccount = account != null;

            return View();
        }
    }
}
