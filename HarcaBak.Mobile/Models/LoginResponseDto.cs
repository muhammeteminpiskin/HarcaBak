namespace HarcaBak.Mobile.Models
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}