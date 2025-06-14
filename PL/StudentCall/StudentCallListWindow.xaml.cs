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
using BO;
using DO;
using PL.StudentCall;

namespace PL.StudentCall
{
    /// <summary>
    /// Interaction logic for StudentCallListWindow.xaml
    /// </summary>
    public partial class StudentCallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.CallStatus? statusCall { get; set; } = BO.CallStatus.None;
        public BO.CallInList? SelectedCall { get; set; }

        public IEnumerable<BO.CallInList> CallsList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallsListProperty); }
            set { SetValue(CallsListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallsList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallsListProperty =
            DependencyProperty.Register("CallsList", typeof(IEnumerable<BO.CallInList>), typeof(StudentCallListWindow), new PropertyMetadata(null));

        public BO.StudentCallField? SelectedSearchOption { get; set; } = null;
        public object SearchValue { get; set; } = string.Empty;
        public BO.StudentCallField? SelectedSortOption { get; set; }

        public StudentCallListWindow()
        {
            QueryCallsList();
            InitializeComponent();
        }

        private void QueryCallsList()
        {
            try
            {
                CallsList = (SelectedSearchOption==null) ?
                            s_bl?.StudentCall.FilterCallsInList().ToList()! : s_bl?.StudentCall.FilterCallsInList(SelectedSearchOption, SearchValue)!;
            }
            catch (BO.BlValidationException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
        }

        private void CallsListObserver()
            => QueryCallsList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.StudentCall.AddObserver(CallsListObserver);
        

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.StudentCall.RemoveObserver(CallsListObserver);

        private void LsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
                new StudentCallWindow(SelectedCall.CallId).Show();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            new StudentCallWindow().Show();
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CallInList call)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the StudentCall with ID {call.CallId}?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.StudentCall.Delete(call.CallId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }


        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CallInList call)
            {
                try
                {
                    s_bl.StudentCall.UpdateTreatmentCancellation((int)call.CallId);
                    //MessageBox.Show("ההקצאה בוטלה ונשלח אימייל.");
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (BO.BlCanNotBeDeletedException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
=> CallsList = s_bl?.StudentCall.FilterCallsInList().ToList()!;

        private void SearchButton_Click(object sender, RoutedEventArgs e) => QueryCallsList();

        private void SortButton_Click(object sender, RoutedEventArgs e)
        => CallsList = s_bl.StudentCall.SortCallsInList(SelectedSortOption).ToList();
    }
}
