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
using Microsoft.Win32;
using Microsoft.Web.WebView2.Core;
using System.Windows.Threading;


namespace PL.StudentCall
{
    /// <summary>
    /// Interaction logic for OpenCallsWindow.xaml
    /// </summary>
    public partial class OpenCallsWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        public BO.OpenCallField? SelectedSearchOption { get; set; } = null;
        public object SearchValue { get; set; } = string.Empty;
        public BO.OpenCallInList? SelectedOpenCall { get; set; }
        public BO.OpenCallField? SelectedSortOption { get; set; }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.StudentCall.AddObserver(CallsListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.StudentCall.RemoveObserver(CallsListObserver);


        public void CallsListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    QueryOpenCall();

                });
        }

        public void QueryOpenCall()
        {
            if (SelectedSearchOption == null || string.IsNullOrEmpty(SearchValue?.ToString()))
            {
                OpenCalls = s_bl.StudentCall.GetOpenCallsForTutor(TutorId).ToList();
                return;
            }
            try
            {
                OpenCalls = s_bl.StudentCall.FilterOpenCalls(TutorId, SelectedSearchOption.Value, SearchValue.ToString()).ToList();
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                this.Close();
            }
            catch (BO.BlCanNotAssignCall ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCall != null)
            {
                Description = SelectedCall.Description ?? "";
                //MapBrowser.Navigate(url);
                InitializeAsync();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
=> OpenCalls = s_bl?.StudentCall.GetOpenCallsForTutor(TutorId).ToList()!;

        private void SearchButton_Click(object sender, RoutedEventArgs e) => QueryOpenCall();

        private void SortButton_Click(object sender, RoutedEventArgs e)
        => OpenCalls = s_bl.StudentCall.SortOpenCalls(TutorId, SelectedSortOption).ToList();

        private async void InitializeAsync()
        {
            var studentCall = s_bl.StudentCall.Read(SelectedCall.Id);
            var tutor = s_bl.Tutor.Read(TutorId);
            string url = $"https://www.google.com/maps/dir/?api=1&origin={tutor.Latitude},{tutor.Longitude}&destination={studentCall.Latitude},{studentCall.Longitude}&travelmode=driving";
            await MyWebView.EnsureCoreWebView2Async(null);
            MyWebView.Source = new Uri(url);
        }
    }
}
