using BlApi;
using BO;
using PL.StudentCall;
using PL.Tutor;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for AdminHomeWindow.xaml
    /// </summary>
    public partial class AdminHomeWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperationClock = null; //stage 7
        private volatile DispatcherOperation? _observerOperationRiskTime = null; //stage 7
        private volatile DispatcherOperation? _observerOperation = null; //stage 7



        public object CallStatusSummaries
        {
            get { return (object)GetValue(CallStatusSummariesProperty); }
            set { SetValue(CallStatusSummariesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallStatusSummaries.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallStatusSummariesProperty =
            DependencyProperty.Register("CallStatusSummaries", typeof(object), typeof(AdminHomeWindow), new PropertyMetadata(null));



        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(AdminHomeWindow), new PropertyMetadata(""));


        public int Interval { get; set; } = 3000;

        public int SelectedStatus { get; set; }

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }

        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(AdminHomeWindow), new PropertyMetadata(false));


        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }


        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AdminHomeWindow), new PropertyMetadata(DateTime.Now));



        public TimeSpan RiskTimeSpan
        {
            get { return (TimeSpan)GetValue(RiskTimeSpanProperty); }
            set { SetValue(RiskTimeSpanProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RiskTimeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RiskTimeSpanProperty =
            DependencyProperty.Register("RiskTimeSpan", typeof(TimeSpan), typeof(AdminHomeWindow), new PropertyMetadata(TimeSpan.Zero));
        private int ManagerId { get; set; }

        public AdminHomeWindow(int managerId)
        {
            ManagerId = managerId;
            ButtonText = "Start Simulator";
            IsSimulatorRunning = false;
            CallStatusSummaries = s_bl.StudentCall.GetCallStatusSummaries();
            InitializeComponent();
        }

        private void CallStatusSummariesObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CallStatusSummaries = s_bl.StudentCall.GetCallStatusSummaries();
                });
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute);
            }
            catch (BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.AdvanceClock(BO.TimeUnit.Year);
            }
            catch (BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.AdvanceClock(BO.TimeUnit.Day);
            }
            catch (BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour);
            }
            catch (BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.AdvanceClock(BO.TimeUnit.Month);
            }
            catch (BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnUpdateRiskTimeSpan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetRiskTimeRange(RiskTimeSpan); 
                MessageBox.Show("Success update risk time span");
            }
            catch (BLTemporaryNotAvailableException ex) { 
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        private void ClockObserver()
        {
            if (_observerOperationClock is null || _observerOperationClock.Status == DispatcherOperationStatus.Completed)
                _observerOperationClock = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetSystemClock();
                });
        }
        private void RiskTimeObserver()
        {
            if (_observerOperationRiskTime is null || _observerOperationRiskTime.Status == DispatcherOperationStatus.Completed)
                _observerOperationRiskTime = Dispatcher.BeginInvoke(() =>
                {
                    RiskTimeSpan = s_bl.Admin.GetRiskTimeRange();
                });
        }

        private void AdminHomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            RiskTimeSpan = s_bl.Admin.GetRiskTimeRange();
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(RiskTimeObserver);
            s_bl.StudentCall.AddObserver(CallStatusSummariesObserver);
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            if (IsSimulatorRunning)
            {
                await Task.Run(() => s_bl.Admin.StopSimulator());
            }
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(RiskTimeObserver);
            s_bl.StudentCall.RemoveObserver(CallStatusSummariesObserver);
            CloseOwnedWindows(this);


        }

        private void CloseOwnedWindows(Window parent)
        {
            foreach (Window child in Application.Current.Windows)
            {
                if (child.Owner == parent)
                {
                    CloseOwnedWindows(child);
                    child.Close();
                }
            }
        }

        private void btnTutors_Click(object sender, RoutedEventArgs e)
        {
            TutorListWindow.ShowWindow(this, ManagerId);
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset the list?",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.ResetDatabase();
                    MessageBox.Show(
                        "The list has been successfully reset.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }catch(BLTemporaryNotAvailableException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred while resetting the list:\n{ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void btnInitDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to initialize the database?\nThis action may overwrite existing data.",
                "Confirm Initialization",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.InitializeDatabase();
                    MessageBox.Show(
                        "Database was initialized successfully.",
                        "Initialization Complete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (BLTemporaryNotAvailableException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred while initializing the database:\n{ex.Message}",
                        "Initialization Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }


        private void btnCalls_Click(object sender, RoutedEventArgs e)
        {
            StudentCallListWindow.ShowWindow(this,ManagerId);
        }

        private async void btnStartOrStopSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                await Task.Run(() => s_bl.Admin.StopSimulator());
                ButtonText = "Start Simulator";
                IsSimulatorRunning = false;
            }
            else
            {
                IsSimulatorRunning = true;
                s_bl.Admin.StartSimulator(Interval); //stage 7
                ButtonText = "Stop Simulator";

            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StudentCallListWindow.ShowWindow(this, ManagerId, (BO.CallStatus)SelectedStatus);
        }
    }
}