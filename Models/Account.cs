using ATM_Simulation_System.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATM_Simulation_System.Models
{
    [Index(nameof(AccountNumber), IsUnique = true)] //Two accounts cannot have the same AccountNumber
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ATM_Simulation_SystemUser User { get; set; }

        [Required]
        [MaxLength(20)]
        public string AccountNumber { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [MaxLength(20)]
        public string AccountType { get; set; }   // Saving / Current

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
