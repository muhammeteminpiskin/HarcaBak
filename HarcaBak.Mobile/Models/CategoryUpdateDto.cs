namespace HarcaBak.Mobile.Models
{
    public class CategoryUpdateDto
    {
        public string Name { get; set; } = string.Empty;

        public int UpdatedByUserId { get; set; }
    }
}