using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(LogInWindow), new PropertyMetadata(""));


        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(LogInWindow), new PropertyMetadata(""));

        private Action<int> Navigate { get; set; }
        public LogInPage(Action<int> navigate)
        {
            Navigate = navigate;
            InitializeComponent();
        }


        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate ID and Password inputs
                if (string.IsNullOrWhiteSpace(Id) || !int.TryParse(Id, out int parsedId))
                {
                    MessageBox.Show("Invalid ID. Please enter a numeric value.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Password cannot be empty.");
                    return;
                }
                // Attempt to log in the user
                BO.Role currentUserRole = s_bl.Tutor.LogIn(parsedId, Password);
                // If login is successful, open the appropriate window based on the role
                if (currentUserRole == BO.Role.Manager)
                {
                    Navigate(parsedId);
                }
                else
                {
                    new TutorHomeWindow(parsedId).Show();
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
            catch (BO.BlValidationException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
        }
    }
}
