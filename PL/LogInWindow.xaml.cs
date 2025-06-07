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
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private string Id { get; set; }
        private string Password { get; set; }

        public LogInWindow()
        {
            InitializeComponent();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            
               BO.Role currentUserRole= s_bl.Tutor.LogIn(int.Parse(Id), Password);
                if (currentUserRole == BO.Role.Manager)
                {

                }
                else
                {

                }



            }
            catch(BO.BlDoesNotExistException ex)
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
