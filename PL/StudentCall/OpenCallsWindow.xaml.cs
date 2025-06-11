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
using BlApi;

namespace PL.StudentCall
{
    /// <summary>
    /// Interaction logic for OpenCallsWindow.xaml
    /// </summary>
    public partial class OpenCallsWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private int TutorId { get; set; }

        public BO.OpenCallField FilterField
        {
            get { return (BO.OpenCallField)GetValue(FilterFieldProperty); }
            set { SetValue(FilterFieldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterField.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterFieldProperty =
            DependencyProperty.Register("FilterField", typeof(BO.OpenCallField), typeof(OpenCallsWindow), new PropertyMetadata(BO.OpenCallField.None));



        public List<BO.OpenCallInList> OpenCalls
        {
            get { return (List<BO.OpenCallInList>)GetValue(OpenCallsProperty); }
            set { SetValue(OpenCallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenCalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenCallsProperty =
            DependencyProperty.Register("OpenCalls", typeof(List<BO.OpenCallInList>), typeof(OpenCallsWindow), new PropertyMetadata(null));


        public OpenCallsWindow(int id)
        {
            TutorId = id;
            OpenCalls = s_bl.StudentCall.GetOpenCallsForTutor(TutorId).ToList();
            InitializeComponent();
        }

        private void FilterOpenCalls(object sender, SelectionChangedEventArgs e)
        {
            OpenCalls = (FilterField == BO.OpenCallField.None) ?
                s_bl.StudentCall.GetOpenCallsForTutor(TutorId).ToList() :
                s_bl.StudentCall.GetOpenCallsForTutor(TutorId).ToList();
        }
    }
}
