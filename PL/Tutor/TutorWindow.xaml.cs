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
using PL.StudentCall;

namespace PL.Tutor
{
    /// <summary>
    /// Interaction logic for TutorWindow.xaml
    /// </summary>
    public partial class TutorWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private int ManagerId { get; set; }
        public BO.Tutor CurrentTutor
        {
            get { return (BO.Tutor)GetValue(CurrentTutorProperty); }
            set { SetValue(CurrentTutorProperty, value); }
        }

        public static readonly DependencyProperty CurrentTutorProperty =
            DependencyProperty.Register("CurrentTutor", typeof(BO.Tutor), typeof(TutorWindow), new PropertyMetadata(null));

        public static string ButtonText { get; set; }
        public static bool IsFromTutorWindow { get; set; }
        public static bool IdIsReadOnly { get; set; }
        public static bool NotHasCallInProgress { get; set; }


        public TutorWindow(int id = 0, bool isFromTutorWindow = false, int managerId = 0)
        {
            ManagerId = managerId;
            ButtonText = id == 0 ? "Add" : "Update";
            CurrentTutor = (id != 0) ? s_bl.Tutor.Read(id)! : new BO.Tutor() { Id = 0, FullName = null, CellNumber = null, Email = null, Password = null, CurrentAddress = null, Latitude = 0, Longitude = 0, Role = BO.Role.None, Active = false, Distance = 0, DistanceType = BO.DistanceType.Walking, TotalCallsHandled = 0, TotalCallsSelfCanceled = 0, TotalCallsExpired = 0 };
            IsFromTutorWindow = isFromTutorWindow;
            IdIsReadOnly = id == 0 ? false : true;
            NotHasCallInProgress = (CurrentTutor.CurrentCallInProgress == null) ? true : false;
            InitializeComponent();
        }

        private void BtnAddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormatValidation();
                if (ButtonText == "Add")
                {
                    s_bl.Tutor.Create(CurrentTutor);
                }
                else
                {
                    if (ManagerId==0)
                    {
                    s_bl.Tutor.Update(CurrentTutor.Id, CurrentTutor);

                    }
                    else
                    {
                        s_bl.Tutor.Update(ManagerId, CurrentTutor);

                    }

                }

                this.Close();

            }
            catch (PL.PlFormatException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
            catch (BO.BlValidationException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, ex.InnerException?.ToString());
            }
        }

        private void FormatValidation()
        {
            if (string.IsNullOrWhiteSpace(CurrentTutor.FullName))
                throw new PL.PlFormatException("Full name cannot be empty.");
            if (string.IsNullOrWhiteSpace(CurrentTutor.CellNumber))
                throw new PL.PlFormatException("Cell number cannot be empty.");
            if (string.IsNullOrWhiteSpace(CurrentTutor.Email))
                throw new PL.PlFormatException("Email cannot be empty.");
            if (CurrentTutor.Role == BO.Role.None)
                throw new PL.PlFormatException("Role cannot be None. Please select a valid role.");
            if (CurrentTutor.Id <= 0)
                throw new PL.PlFormatException("ID must be a positive integer.");
            if (CurrentTutor.FullName.Length < 2 || CurrentTutor.FullName.Length > 100)
                throw new PL.PlFormatException($"Full name '{CurrentTutor.FullName}' must be between 2 and 100 characters.");

        }


        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you want to delete the tutor with ID {CurrentTutor.Id}", "ok", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Tutor.Delete(CurrentTutor.Id);
                    this.Close();
                }
                catch (BO.BlCanNotBeDeletedException ex)
                {
                    MessageBox.Show(ex.Message, ex.InnerException?.ToString());
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(ex.Message, ex.InnerException?.ToString());
                }
            }
        }
        private void BtnDisplayCall_Click(object sender, RoutedEventArgs e)
        {
            var studentCallWindow = new StudentCallWindow(CurrentTutor.CurrentCallInProgress!.CallId, true);
            studentCallWindow.Owner = this;
            studentCallWindow.Show();
        }
        private void TutorObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentTutor!.Id;
                    CurrentTutor = null;
                    CurrentTutor = s_bl.Tutor.Read(id);
                });

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentTutor!.Id != 0)
                s_bl.Tutor.AddObserver(CurrentTutor.Id, TutorObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentTutor!.Id != 0)
                s_bl.Tutor.RemoveObserver(CurrentTutor.Id, TutorObserver);
        }
    }

}
