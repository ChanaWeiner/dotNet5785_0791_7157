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
using BO;
using DO;
using PL.StudentCall;
using PL.Tutor;

namespace PL.StudentCall
{
    /// <summary>
    /// Interaction logic for StudentCallListWindow.xaml
    /// </summary>
    public partial class StudentCallListWindow : Window
    {
        private static StudentCallListWindow? s_instance;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
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

        private int ManagerId { get; set; }

        public static void ShowWindow(Window owner, int managerId, BO.CallStatus status = BO.CallStatus.None)
        {

            if (s_instance == null)
            {
                s_instance = new StudentCallListWindow(managerId, status);
                s_instance.Owner = owner;
                s_instance.Closed += (_, _) => s_instance = null;
                s_instance.Show();
            }
            else
            {
                if (status != BO.CallStatus.None|| s_instance.SearchValue != string.Empty)
                {
                    s_instance.Close();
                    s_instance = new StudentCallListWindow(managerId, status);
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
        }
        private StudentCallListWindow(int managerId, BO.CallStatus status = BO.CallStatus.None)
        {
            if (status != BO.CallStatus.None)
            {
                SearchValue = status;
                SelectedSearchOption = BO.StudentCallField.Status;
            }
            ManagerId = managerId;
            QueryCallsList();
            InitializeComponent();
        }

        private void QueryCallsList()
        {
            try
            {
                CallsList = (SelectedSearchOption == null) ?
                            s_bl?.StudentCall.FilterCallsInList().ToList()! : s_bl?.StudentCall.FilterCallsInList(SelectedSearchOption, SearchValue)!;
            }
            catch (BO.BlValidationException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
        }

        private void CallsListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    QueryCallsList();
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.StudentCall.AddObserver(CallsListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.StudentCall.RemoveObserver(CallsListObserver);

        private void LsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                var studentCallWindow = new StudentCallWindow(SelectedCall.CallId, false, ManagerId);
                studentCallWindow.Owner = this;
                studentCallWindow.Show();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var studentCallWindow = new StudentCallWindow();
            studentCallWindow.Owner = this;
            studentCallWindow.Show();
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
                        s_bl.StudentCall.Delete(ManagerId, call.CallId);

                    }
                    catch (BO.BlCanNotBeDeletedException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (BO.BlAccessDeniedException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch(BO.BLTemporaryNotAvailableException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (call.Id != null)
                        s_bl.StudentCall.UpdateTreatmentCancellation((int)call.Id, ManagerId);
                    else
                        MessageBox.Show("Cannot cancel treatment because the call is open");
                    //MessageBox.Show("ההקצאה בוטלה ונשלח אימייל.");
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (BO.BlCanNotUpdateTreatment ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedSearchOption = null;
            SearchValue = string.Empty;
            CallsList = s_bl?.StudentCall.FilterCallsInList().ToList()!;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => QueryCallsList();

        private void SortButton_Click(object sender, RoutedEventArgs e)
        => CallsList = s_bl.StudentCall.SortCallsInList(SelectedSortOption).ToList();
    }
}
