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

        public StudentCallListWindow()
        {
            InitializeComponent();
        }

        private void FilterCalls(object sender, SelectionChangedEventArgs e) => QueryCallsList();
        private void QueryCallsList()
        {
            try
            {
                CallsList = (statusCall == BO.CallStatus.None) ?
                            s_bl?.StudentCall.FilterCallsInList()! : s_bl?.StudentCall.FilterCallsInList(c=>c.Status == statusCall)!;

            }
            catch (BO.BlValidationException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
        }

        private void CallsListObserver()
            => QueryCallsList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.StudentCall.AddObserver(CallsListObserver);
        }

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
                        QueryCallsList();
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
                    s_bl.StudentCall.UpdateTreatmentCancellation((int)call.Id);
                    MessageBox.Show("ההקצאה בוטלה ונשלח אימייל.");
                    QueryCallsList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
