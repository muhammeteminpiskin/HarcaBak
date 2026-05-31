using HarcaBak.Mobile.Views;

namespace HarcaBak.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(TransactionListPage), typeof(TransactionListPage));
            Routing.RegisterRoute(nameof(TransactionAddPage), typeof(TransactionAddPage));
            Routing.RegisterRoute(nameof(TransactionEditPage), typeof(TransactionEditPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
        }
    }
}