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
        private bool hasCallInProgress{get;set;}
        private bool noCallInProgress{get;set;}
        private bool hasCallsHistory{get;set;}
        private int TutorId{get;set; }

        public TutorHomeWindow(int id = 309141281)
        {
            BO.Tutor tutor=s_bl.Tutor.Read(id);
            hasCallInProgress = tutor.CurrentCallInProgress != null;
            noCallInProgress = !hasCallInProgress;
            hasCallsHistory = s_bl.StudentCall.FilterCallsInList(BO.StudentCallField.Id,id).Count()!=0;
            TutorId = id;

            InitializeComponent();
        }

        private void BtnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            new StudentCallListWindow().Show();
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
            new CallsHistoryWindow().Show();
        }
    }
}
