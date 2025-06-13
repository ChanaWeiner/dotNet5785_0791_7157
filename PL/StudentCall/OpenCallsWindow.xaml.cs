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
using BO;

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


 
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(OpenCallsWindow), new PropertyMetadata(""));

        public string MapUrl
        {
            get { return (string)GetValue(MapUrlProperty); }
            set { SetValue(MapUrlProperty, value); }
        }

        public static readonly DependencyProperty MapUrlProperty =
            DependencyProperty.Register("MapUrl", typeof(string), typeof(OpenCallsWindow), new PropertyMetadata(null));

        public BO.OpenCallInList SelectedCall
        {
            get { return (BO.OpenCallInList)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(BO.OpenCallInList), typeof(OpenCallsWindow), new PropertyMetadata(null));


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

        private void AssignCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.StudentCall.AssignCallToTutor(TutorId, SelectedCall.Id);
            }
            catch(BO.BlCanNotAssignCall ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCall != null)
            {
                Description = SelectedCall.Description ?? "";

                if (!string.IsNullOrEmpty(SelectedCall.FullAddress))
                {
                    string encodedAddress = Uri.EscapeDataString(SelectedCall.FullAddress);
                    MapUrl = $"https://www.google.com/maps?q={encodedAddress}&output=embed";
                    MapBrowser.Navigate(MapUrl);
                }
                else
                {
                    MapUrl = "";
                }
            }
        }


    }
}
