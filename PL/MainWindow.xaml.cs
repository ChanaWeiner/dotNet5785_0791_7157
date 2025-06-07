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

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        public DateTime CurrentTime
        {
            get { return (DateTime )GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        } 


        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime ), typeof(MainWindow), new PropertyMetadata(DateTime.Now));



        public TimeSpan RiskTimeSpan
        {
            get { return (TimeSpan)GetValue(RiskTimeSpanProperty); }
            set { SetValue(RiskTimeSpanProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RiskTimeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RiskTimeSpanProperty =
            DependencyProperty.Register("RiskTimeSpan", typeof(TimeSpan), typeof(MainWindow) ,new PropertyMetadata(TimeSpan.Zero));


        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Year);

        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Day);

        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour);


        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Month);

        }

        private void btnUpdateRiskTimeSpan_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskTimeRange(RiskTimeSpan);

        }
        private void clockObserver()=> CurrentTime = s_bl.Admin.GetSystemClock();
        private void configObserver() => RiskTimeSpan = s_bl.Admin.GetRiskTimeRange();
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            RiskTimeSpan = s_bl.Admin.GetRiskTimeRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        private void btnTutors_Click(object sender, RoutedEventArgs e)
        {
            new TutorListWindow().Show();
        }

        private void BtnResetDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ResetDatabase();
        }

        private void btnInitDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.InitializeDatabase();
        }

        private void btnCalls_Click(object sender, RoutedEventArgs e)
        {
            new StudentCallListWindow().Show();
        }
    }
}