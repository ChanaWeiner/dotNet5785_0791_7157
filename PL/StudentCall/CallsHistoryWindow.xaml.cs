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

namespace PL.StudentCall
{
    /// <summary>
    /// Interaction logic for CallsHistoryWindow.xaml
    /// </summary>
    public partial class CallsHistoryWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.EndOfTreatment EndType
        {
            get { return (BO.EndOfTreatment)GetValue(EndTypeProperty); }
            set { SetValue(EndTypeProperty, value); }
        }

        public static readonly DependencyProperty EndTypeProperty =
            DependencyProperty.Register("EndType", typeof(BO.EndOfTreatment), typeof(CallsHistoryWindow), new PropertyMetadata(BO.EndOfTreatment.None));

        private int TutorId { get; set; } // Tutor ID

        public List<ClosedCallInList> ClosedCallInLists
        {
            get { return (List<ClosedCallInList>)GetValue(ClosedCallInListsProperty); }
            set { SetValue(ClosedCallInListsProperty, value); }
        }

        public static readonly DependencyProperty ClosedCallInListsProperty =
            DependencyProperty.Register(
                "ClosedCallInLists",
                typeof(List<ClosedCallInList>),                 
                typeof(CallsHistoryWindow),                     
                new PropertyMetadata(null));                    


        public CallsHistoryWindow(int id)
        {
            TutorId = id;
            ClosedCallInLists = s_bl.StudentCall.GetClosedCallsForTutor(id).ToList();
            InitializeComponent();
        }

        private void FilterClosedCalls(object sender, SelectionChangedEventArgs e)
        {
            ClosedCallInLists = (EndType==BO.EndOfTreatment.None)? s_bl.StudentCall.GetClosedCallsForTutor(TutorId, null).ToList() :
                s_bl.StudentCall.GetClosedCallsForTutor(TutorId, c => c.EndType == EndType).ToList();
        }
        private void CallsHistorybserver()
        {
            ClosedCallInLists = s_bl.StudentCall.GetClosedCallsForTutor(TutorId).ToList();

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                s_bl.Tutor.AddObserver(TutorId, CallsHistorybserver);
        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.StudentCall.RemoveObserver(CallsHistorybserver);
    }
}
