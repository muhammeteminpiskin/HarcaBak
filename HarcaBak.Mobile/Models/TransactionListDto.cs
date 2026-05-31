namespace HarcaBak.Mobile.Models
{
    public class TransactionListDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public DateTime Date { get; set; }

        public TransactionType Type { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string TypeText => Type == TransactionType.Income ? "Gelir" : "Gider";

        public string DateText => Date.ToString("dd.MM.yyyy");

        public string AmountText => $"{Amount:C}";
    }
}