using BO;
using DO;
using PL.StudentCall;
using PL.Tutor;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for TutorHome.xaml
    /// </summary>
    public partial class TutorHomeWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7


        public bool HasCallInProgress
        {
            get { return (bool)GetValue(HasCallInProgressProperty); }
            set { SetValue(HasCallInProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasCallInProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasCallInProgressProperty =
            DependencyProperty.Register("HasCallInProgress", typeof(bool), typeof(TutorHomeWindow), new PropertyMetadata(false));


        public bool NoCallInProgress
        {
            get { return (bool)GetValue(NoCallInProgressProperty); }
            set { SetValue(NoCallInProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoCallInProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoCallInProgressProperty =
            DependencyProperty.Register("NoCallInProgress", typeof(bool), typeof(TutorHomeWindow), new PropertyMetadata(false));

        public bool HasCallsHistory { get; set; }
        private int TutorId { get; set; }

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(Page), typeof(TutorHomeWindow), new PropertyMetadata(null));

        public Page CurrentPage
        {
            get => (Page)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public TutorHomeWindow(int id )
        {
            try
            {
                BO.Tutor tutor = s_bl.Tutor.Read(id);
                HasCallInProgress = tutor.CurrentCallInProgress != null;
                NoCallInProgress = !HasCallInProgress;
                HasCallsHistory = s_bl.StudentCall.FilterCallsInList().Count() != 0;
                TutorId = id;

                InitializeComponent();
                if (HasCallInProgress)
                {
                    CurrentPage = new CurrentCallPage(tutor.Id, tutor.CurrentCallInProgress!.CallId, tutor.CurrentCallInProgress.Id);
                }
            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show($"Tutor with ID {id} does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayCurrentCallObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    var tutor = s_bl.Tutor.Read(TutorId);
                    HasCallInProgress = tutor.CurrentCallInProgress != null;
                    NoCallInProgress = !HasCallInProgress;
                    if (HasCallInProgress)
                    {
                        CurrentPage = new CurrentCallPage(tutor.Id, tutor.CurrentCallInProgress!.CallId, tutor.CurrentCallInProgress.Id);
                    }
                    else
                    {
                        CurrentPage = null;
                    }
                });
        }

        private void BtnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            new OpenCallsWindow(TutorId).Show();
        }

        private void BtnMyDetails_Click(object sender, RoutedEventArgs e)
        {
            new TutorWindow(TutorId, true).Show();
        }

        private void BtnCallsHistory_Click(object sender, RoutedEventArgs e)
        {
            new CallsHistoryWindow(TutorId).Show();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new LogInWindow().Show();
        }

        private void BtnCurrentCall_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Tutor.AddObserver(TutorId, DisplayCurrentCallObserver);
            s_bl.StudentCall.AddObserver(DisplayCurrentCallObserver);

        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Tutor.RemoveObserver(TutorId, DisplayCurrentCallObserver);


    }
}
