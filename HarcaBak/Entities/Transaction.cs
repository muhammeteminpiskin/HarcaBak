using System.ComponentModel.DataAnnotations.Schema;

namespace HarcaBak.Entities
{
    public class Transaction: BaseEntity
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public int UserId { get; set; }

        // Navigation Properties
        public Category Category { get; set; }
        public User User { get; set; }
    }
}
