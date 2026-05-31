namespace HarcaBak.Mobile.Models
{
    public class TransactionCreateDto
    {
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public DateTime Date { get; set; }

        public TransactionType Type { get; set; }

        public int CategoryId { get; set; }

        public int UserId { get; set; }
    }
}