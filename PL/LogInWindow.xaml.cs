using PL.StudentCall;
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
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        
        public Page CurrentPage
        {
            get { return (Page)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        public static readonly DependencyProperty CurrentPageProperty =
    DependencyProperty.Register("CurrentPage", typeof(Page), typeof(LogInWindow), new PropertyMetadata(null));




        public LogInWindow()
        {
            CurrentPage = new LogInPage((id) => NavigateTo(id));
            InitializeComponent();
        }

        public void NavigateTo(int id)
        {
            CurrentPage = new AdminButtonsPage(id);
        }

    }
}
