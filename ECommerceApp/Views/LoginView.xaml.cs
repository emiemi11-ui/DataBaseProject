using ECommerceApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ECommerceApp.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        /// <summary>
        /// Handles PasswordBox password change.
        /// WPF PasswordBox doesn't support binding for security reasons,
        /// so we handle it in code-behind.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
