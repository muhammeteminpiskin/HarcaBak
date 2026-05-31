namespace HarcaBak.Mobile.Helpers
{
    public static class SessionManager
    {
        public static int UserId { get; private set; }

        public static string Name { get; private set; } = string.Empty;

        public static string Email { get; private set; } = string.Empty;

        public static bool IsLoggedIn => UserId > 0;

        public static void SetUser(int userId, string name, string email)
        {
            UserId = userId;
            Name = name;
            Email = email;
        }

        public static void Clear()
        {
            UserId = 0;
            Name = string.Empty;
            Email = string.Empty;
        }
    }
}