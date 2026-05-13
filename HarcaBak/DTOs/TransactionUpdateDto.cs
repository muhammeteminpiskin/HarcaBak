using HarcaBak.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarcaBak.DTOs
{
    public class TransactionUpdateDto
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public int CategoryId { get; set; }
    }
}
