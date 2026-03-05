using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATM_Simulation_System.Models
{
    [Index(nameof(CardNumber), IsUnique = true)] //Two Card cannot have the same CardNumber
    public class Card
    {
        [Key]
        public int CardId { get; set; }

        [Required]
        [MaxLength(16)]
        public string CardNumber { get; set; }

        [Required]
        public string PinHash { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }
    }
}
