namespace HarcaBak.Mobile.Models
{
    public class TransactionSummaryDto
    {
        public int UserId { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal TotalIncome { get; set; }

        public decimal Balance { get; set; }
    }
}