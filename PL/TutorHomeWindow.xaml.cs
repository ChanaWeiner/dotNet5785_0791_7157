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

namespace PL
{
    /// <summary>
    /// Interaction logic for TutorHome.xaml
    /// </summary>
    public partial class TutorHomeWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public bool HasCallInProgress{get;set;}
        public bool NoCallInProgress{get;set;}
        public bool HasCallsHistory{get;set;}
        private int TutorId{get;set; }

        public TutorHomeWindow() : this(265383422) { }
        public TutorHomeWindow(int id = 265383422)
        {
            try
            {
                BO.Tutor tutor = s_bl.Tutor.Read(id);
                HasCallInProgress = tutor.CurrentCallInProgress != null;
                NoCallInProgress = !HasCallInProgress;
                HasCallsHistory = s_bl.StudentCall.GetCalls().Count() != 0;
                TutorId = id;

                InitializeComponent();
            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show($"Tutor with ID {id} does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }          
        }

        private void BtnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            new OpenCallsWindow(TutorId).Show();
        }

        private void BtnMyDetails_Click(object sender, RoutedEventArgs e)
        {
            new TutorWindow(TutorId,true).Show();
        }

        private void BtnCurrentCall_Click(object sender, RoutedEventArgs e)
        {
            new StudentCallWindow().Show();
        }

        private void BtnCallsHistory_Click(object sender, RoutedEventArgs e)
        {
            new CallsHistoryWindow(TutorId).Show();
        }
    }
}
