using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATM_Simulation_System.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; }  // Deposit / Withdraw

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal BalanceAfterTransaction { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
