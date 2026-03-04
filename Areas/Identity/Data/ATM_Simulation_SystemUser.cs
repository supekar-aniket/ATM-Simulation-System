using ATM_Simulation_System.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATM_Simulation_System.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ATM_Simulation_SystemUser class
public class ATM_Simulation_SystemUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime DateAndTime { get; set; } = DateTime.Now;

    public ICollection<Account> Accounts { get; set; }
}

