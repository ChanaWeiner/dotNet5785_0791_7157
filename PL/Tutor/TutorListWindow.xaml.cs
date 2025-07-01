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
using System.Windows.Threading;
using PL.StudentCall;

namespace PL.Tutor
{
    /// <summary>
    /// Interaction logic for TutorListWindow.xaml
    /// </summary>
    public partial class TutorListWindow : Window
    {
        private static TutorListWindow? s_instance;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        public BO.TutorField? SelectedSearchOption { get; set; } = null;
        public object SearchValue { get; set; } = string.Empty;
        public BO.TutorInList? SelectedTutor { get; set; }
        public BO.TutorField? SelectedSortOption { get; set; }
        private int ManagerId{get;set;}
        public IEnumerable<BO.TutorInList> TutorsList
        {
            get { return (IEnumerable<BO.TutorInList>)GetValue(TutorsListProperty); }
            set { SetValue(TutorsListProperty, value); }
        }

        public static readonly DependencyProperty TutorsListProperty =
            DependencyProperty.Register("TutorsList", typeof(IEnumerable<BO.TutorInList>), typeof(TutorListWindow), new PropertyMetadata(null));

        public static void ShowWindow(Window owner,int managerId)
        {
            if (s_instance == null)
            {
                s_instance = new TutorListWindow(managerId);
                s_instance.Owner = owner; 
                s_instance.Closed += (_, _) => s_instance = null;
                s_instance.Show();
            }
            else
            {
                if (s_instance.WindowState == WindowState.Minimized)
                    s_instance.WindowState = WindowState.Normal;

                s_instance.Activate();
            }
        }

        private TutorListWindow(int managerId)
        {
            ManagerId = managerId;
            QueryTutorList();
            InitializeComponent();
        }

        private void FilterTutors(object sender, SelectionChangedEventArgs e) => QueryTutorList();
        private void QueryTutorList()
        => TutorsList = (SelectedSearchOption == null) ?
              s_bl?.Tutor.FilterTutorsInList()! : s_bl?.Tutor.FilterTutorsInList(SelectedSearchOption,SearchValue)!;

        private void TutorListObserver()
        {
            
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    QueryTutorList();
                });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Tutor.AddObserver(TutorListObserver);
        

        private void Window_Closed(object sender, EventArgs e)
        => s_bl.Tutor.RemoveObserver(TutorListObserver);

        private void LsvTutorsList_MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            var tutorWindow = new TutorWindow(SelectedTutor!.Id,false, ManagerId);
            tutorWindow.Owner = this;
            tutorWindow.Show();

        }
        

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var tutorWindow = new TutorWindow();
            tutorWindow.Owner = this;
            tutorWindow.Show();
        }
       
        
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedSearchOption = null;
            SearchValue = string.Empty;
            TutorsList = s_bl?.Tutor.FilterTutorsInList().ToList()!;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => QueryTutorList();

        private void SortButton_Click(object sender, RoutedEventArgs e)
        => TutorsList = s_bl.Tutor.SortTutorsInList(SelectedSortOption);
    }
}
