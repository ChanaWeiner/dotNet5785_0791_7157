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
using PL.StudentCall;

namespace PL.Tutor
{
    /// <summary>
    /// Interaction logic for TutorWindow.xaml
    /// </summary>
    public partial class TutorWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Tutor CurrentTutor
        {
            get { return (BO.Tutor)GetValue(CurrentTutorProperty); }
            set { SetValue(CurrentTutorProperty, value); }
        }

        public static readonly DependencyProperty CurrentTutorProperty =
            DependencyProperty.Register("CurrentTutor", typeof(BO.Tutor), typeof(TutorWindow), new PropertyMetadata(null));

        public static string ButtonText { get; set; }

        public TutorWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            CurrentTutor = (id != 0) ? s_bl.Tutor.Read(id)! : new BO.Tutor() { Id = 0, FullName = null, CellNumber = null, Email = null, Password = null, CurrentAddress = null, Latitude = 0, Longitude = 0, Role = BO.Role.None, Active = false, Distance = 0, DistanceType = BO.DistanceType.Walking, TotalCallsHandled = 0, TotalCallsSelfCanceled = 0, TotalCallsExpired = 0 };
            InitializeComponent();
        }

        private void btnAddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Tutor.Create(CurrentTutor);
                    this.Close();
                }

                else
                    s_bl.Tutor.Update(CurrentTutor.Id, CurrentTutor);
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
        private void btnDelete_Click(object sender, RoutedEventArgs e)
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

        private void btnDisplayCall_Click(object sender, RoutedEventArgs e)
        {
            new StudentCallWindow(CurrentTutor.CurrentCallInProgress!.CallId,true)
            {
                Owner = this
            }.Show();
        }
        private void tutorObserver()
        {
            int id = CurrentTutor!.Id;
            CurrentTutor = null;
            CurrentTutor = s_bl.Tutor.Read(id);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentTutor!.Id != 0)
                s_bl.Tutor.AddObserver(CurrentTutor.Id, tutorObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Tutor.RemoveObserver(tutorObserver);
    }

}
