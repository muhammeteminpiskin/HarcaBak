namespace HarcaBak.Mobile.Models
{
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
    }
}