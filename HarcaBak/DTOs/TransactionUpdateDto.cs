using HarcaBak.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarcaBak.DTOs
{
    public class TransactionUpdateDto
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }
}
