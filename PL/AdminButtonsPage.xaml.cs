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
    /// Interaction logic for AdminButtonsPage.xaml
    /// </summary>
    public partial class AdminButtonsPage : Page
    {
        private int Id { get; set; }
        public AdminButtonsPage(int id)
        {
            Id = id;
            InitializeComponent();
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            new AdminHomeWindow().Show();
        }
        private void Tutor_Click(object sender, RoutedEventArgs e)
        {
            new TutorHomeWindow(Id).Show();
        }
    }
}
