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
using BO;
using PL.StudentCall;
using PL.Tutor;

namespace PL.StudentCall
{
    /// <summary>
    /// Interaction logic for StudentCallWindow.xaml
    /// </summary>
    public partial class StudentCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        public BO.StudentCall CurrentStudentCall
        {
            get { return (BO.StudentCall)GetValue(CurrentStudentCallProperty); }
            set { SetValue(CurrentStudentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentStudentCallProperty =
            DependencyProperty.Register("CurrentStudentCall", typeof(BO.StudentCall), typeof(StudentCallWindow), new PropertyMetadata(null));

        public string ButtonText { get; set; }
        public bool IsFromTutor { get; set; }
        public bool IsNotFromTutor { get; set; }
        public bool IsInTreatment { get; set; }
        public bool IsClosedOrExpired { get; set; }
        public bool IsTotalyReadOnly { get; set; }
        public bool IsFinalTimeReadOnly { get; set; }
        private int ManagerId { get; set; } = 0; // For future use if needed

        private void btnAddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormatValidation(); 

                if (ButtonText == "Add")
                {
                    s_bl.StudentCall.Create(CurrentStudentCall);
                    this.Close();
                }
                else
                {
                    s_bl.StudentCall.Update(CurrentStudentCall);
                    this.Close();
                }
            }
            catch (PL.PlFormatException ex)
            {
                MessageBox.Show("Formatting Error: " + ex.Message, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch(BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (BO.BlValidationException ex)
            {
                MessageBox.Show(
                    "Some of the entered information is invalid or missing. Please check the details and try again.\n\nDetails: " + ex.Message,
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }

            catch (BO.BlAlreadyExistsException ex)
            {
                MessageBox.Show(
                    "This student call already exists in the system and cannot be added again.\n\nDetails: " + ex.Message,
                    "Duplicate Entry",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(
                    "The student call you're trying to update does not exist. It may have been removed.\n\nDetails: " + ex.Message,
                    "Item Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An unexpected error occurred. Please try again or contact support.\n\nDetails: " + ex.Message,
                    "Unexpected Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you want to delete the StudentCall with ID {CurrentStudentCall.Id}", "ok", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.StudentCall.Delete(ManagerId, CurrentStudentCall.Id);
                    this.Close();
                }
                catch (BLTemporaryNotAvailableException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                catch (BO.BlCanNotBeDeletedException ex)
                {
                    MessageBox.Show(ex.Message, ex.InnerException?.ToString());
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(ex.Message, ex.InnerException?.ToString());
                }
                catch(BO.BlAccessDeniedException ex)
                {
                    MessageBox.Show("You do not have permission to delete this student call.\n\nDetails: " + ex.Message, "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred while trying to delete the student call.\n\nDetails: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void StudentCallObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentStudentCall!.Id;
                    CurrentStudentCall = null;
                    CurrentStudentCall = s_bl.StudentCall.Read(id);
                    if (CurrentStudentCall!.Status != BO.CallStatus.InProgress || CurrentStudentCall.Status != BO.CallStatus.InProgressAtRisk)
                    {
                        this.Close();
                    }
                });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentStudentCall!.Id != 0)
                s_bl.StudentCall.AddObserver(CurrentStudentCall.Id, StudentCallObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.StudentCall.RemoveObserver(CurrentStudentCall.Id, StudentCallObserver);
    
        public StudentCallWindow(int id=0,bool isFromTutor=false,int managerId=0)
        {
            ManagerId = managerId;
            IsFromTutor = isFromTutor;
            IsNotFromTutor = !isFromTutor;

            if(IsNotFromTutor)
            {
                ButtonText = id == 0 ? "Add" : "Update";
            }
            CurrentStudentCall = (id != 0) ? s_bl.StudentCall.Read(id)! : new BO.StudentCall
            {
                Id = 0,
                FullName = string.Empty,
                CellNumber = string.Empty,
                Email = string.Empty,
                Subject = BO.Subjects.None,
                Description = string.Empty,
                FullAddress = string.Empty,
                OpenTime = DateTime.Now,
                FinalTime = null,
                Status = BO.CallStatus.Open,
                CallsAssignInList = new List<BO.CallAssignInList>()
            };

            switch(CurrentStudentCall.Status)
            {
                case BO.CallStatus.InProgress:
                case BO.CallStatus.InProgressAtRisk:
                    IsInTreatment = true;
                    IsClosedOrExpired = false;
                    break;
                case BO.CallStatus.Expired:
                case BO.CallStatus.Closed:
                    IsInTreatment = false;
                    IsClosedOrExpired = true;
                    break;
                default:
                    IsInTreatment = false;
                    IsClosedOrExpired = false;
                    break;
            }
            IsFinalTimeReadOnly = (IsFromTutor || IsClosedOrExpired);
            IsTotalyReadOnly  = (IsFromTutor || IsClosedOrExpired || IsInTreatment);
            InitializeComponent();
        }
        private void FormatValidation()
        {
            if (string.IsNullOrWhiteSpace(CurrentStudentCall.FullName))
                throw new PL.PlFormatException("Full name cannot be empty.");

            if (CurrentStudentCall.FullName.Length < 2 || CurrentStudentCall.FullName.Length > 100)
                throw new PL.PlFormatException($"Full name '{CurrentStudentCall.FullName}' must be between 2 and 100 characters.");

            if (string.IsNullOrWhiteSpace(CurrentStudentCall.CellNumber))
                throw new PL.PlFormatException("Cell number cannot be empty.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(CurrentStudentCall.CellNumber, @"^0\d{1,2}-?\d{6,7}$"))
                throw new PL.PlFormatException("Cell number format is invalid. Expected: 050-1234567");

            if (string.IsNullOrWhiteSpace(CurrentStudentCall.Email))
                throw new PL.PlFormatException("Email cannot be empty.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(CurrentStudentCall.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new PL.PlFormatException("Email format is invalid.");

            if (CurrentStudentCall.Subject == BO.Subjects.None)
                throw new PL.PlFormatException("You must choose a subject.");

            if (string.IsNullOrWhiteSpace(CurrentStudentCall.Description))
                throw new PL.PlFormatException("Description cannot be empty.");

            if (string.IsNullOrWhiteSpace(CurrentStudentCall.FullAddress))
                throw new PL.PlFormatException("Address cannot be empty.");

            if (CurrentStudentCall.OpenTime > DateTime.Now)
                throw new PL.PlFormatException("Open time cannot be in the future.");
        }

    }
}
