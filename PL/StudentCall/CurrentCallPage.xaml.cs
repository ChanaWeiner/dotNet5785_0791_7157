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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL.StudentCall
{
    /// <summary>
    /// Interaction logic for CurrentCallPage.xaml
    /// </summary>
    public partial class CurrentCallPage : Page
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.StudentCall CurrentStudentCall
        {
            get { return (BO.StudentCall)GetValue(CurrentStudentCallProperty); }
            set { SetValue(CurrentStudentCallProperty, value); }
        }
        public static readonly DependencyProperty CurrentStudentCallProperty =
            DependencyProperty.Register("CurrentStudentCall", typeof(BO.StudentCall), typeof(CurrentCallPage), new PropertyMetadata(null));

        public int TutorId { get; set; }
        public int CallId { get; set; }
        public int AssignmentId { get; set; }
        public CurrentCallPage(int tuturId, int callId, int assignmentId)
        {
            //תאתחל אותם
            TutorId = tuturId;
            CallId = callId;
            AssignmentId = assignmentId;

            InitializeComponent();
            try
            {
                CurrentStudentCall = s_bl.StudentCall.Read(callId);

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while loading the current call. Please try again or contact support.\n\nDetails: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Logic to handle cancel action  
            MessageBox.Show("Cancel button clicked.");
            //אני רוצה להפעיל פונקציה בBL
            //תעטוף את זה בtry catch
            try
            {
                s_bl.StudentCall.UpdateTreatmentCancellation(AssignmentId,TutorId);
                MessageBox.Show("Treatment cancelled successfully.");
            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show("The call does not exist or has already been processed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlCanNotUpdateTreatment ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEndTreatment_Click(object sender, RoutedEventArgs e)
        {
            // Logic to handle end treatment action  
            //תעשה אותו רעיון כאן
            MessageBox.Show("End Treatment button clicked.");
            try
            {
                s_bl.StudentCall.UpdateTreatmentCompletion(TutorId,AssignmentId);
                MessageBox.Show("Treatment ended successfully.");
            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show("The call does not exist or has already been processed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlCanNotUpdateTreatment ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
