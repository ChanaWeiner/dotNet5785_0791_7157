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

namespace PL.Tutor
{
    /// <summary>
    /// Interaction logic for TutorListWindow.xaml
    /// </summary>
    public partial class TutorListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Role role { get; set; } = BO.Role.None;
        public BO.TutorInList? SelectedTutor { get; set; }

        public IEnumerable<BO.TutorInList> TutorsList
        {
            get { return (IEnumerable<BO.TutorInList>)GetValue(TutorsListProperty); }
            set { SetValue(TutorsListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TutorsList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TutorsListProperty =
            DependencyProperty.Register("TutorsList", typeof(IEnumerable<BO.TutorInList>), typeof(TutorListWindow), new PropertyMetadata(null));


        public TutorListWindow()
        {
            InitializeComponent();
        }

        private void FilterTutors(object sender, SelectionChangedEventArgs e) => queryTutorList();
        private void queryTutorList()
    => TutorsList = (role == BO.Role.None) ?
              s_bl?.Tutor.FilterTutorsInList()! : s_bl?.Tutor.FilterTutorsInList(BO.TutorField.Role, role)!;

        private void tutorListObserver()
            => queryTutorList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             s_bl.Tutor.AddObserver(tutorListObserver);
            MessageBox.Show("Observer registered");

        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Tutor.RemoveObserver(tutorListObserver);

        private void lsvTutorsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedTutor != null)
                new TutorWindow(SelectedTutor.Id).Show();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            new TutorWindow().Show();
        }
    }
}
