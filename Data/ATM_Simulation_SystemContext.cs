using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ATM_Simulation_System.Models;

namespace ATM_Simulation_System.Data
{
    public class ATM_Simulation_SystemContext : DbContext
    {
        public ATM_Simulation_SystemContext (DbContextOptions<ATM_Simulation_SystemContext> options)
            : base(options)
        {
        }

        public DbSet<ATM_Simulation_System.Models.Account> Account { get; set; } = default!;
    }
}
