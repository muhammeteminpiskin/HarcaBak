namespace HarcaBak.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public List<Transaction> Transactions { get; set; } = new();

    }
}
