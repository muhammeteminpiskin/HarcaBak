namespace HarcaBak.DTOs
{
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
    }
}
