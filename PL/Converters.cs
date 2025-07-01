using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using PL.Tutor;

namespace PL
{

    class ConvertUpdateToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            return str == "Update" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BoolInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return !b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ConvertBoolToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ConvertCurrentCallToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.CallInProgress currentCall = value as BO.CallInProgress;
            return currentCall != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertDeleteToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Update" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : false;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : false;
    }
    public class NoAssignmentsToVisibilityConverter : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int callId = (int)value;

            return !s_bl.StudentCall.hasAssignments(callId) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CallStatus status)
            {
                switch (status)
                {
                    case BO.CallStatus.Open:
                        return new SolidColorBrush(Color.FromRgb(173, 216, 230)); // LightBlue
                    case BO.CallStatus.InProgress:
                        return new SolidColorBrush(Color.FromRgb(144, 238, 144)); // LightGreen
                    case BO.CallStatus.Closed:
                        return new SolidColorBrush(Color.FromRgb(211, 211, 211)); // LightGray
                    case BO.CallStatus.Expired:
                        return new SolidColorBrush(Color.FromRgb(255, 182, 193)); // LightPink
                    case BO.CallStatus.OpenInRisk:
                        return new SolidColorBrush(Color.FromRgb(255, 228, 181)); // Moccasin
                    case BO.CallStatus.InProgressAtRisk:
                        return new SolidColorBrush(Color.FromRgb(221, 160, 221)); // Plum
                    case BO.CallStatus.None:
                        return Brushes.Transparent;
                    default:
                        return Brushes.Transparent;
                }
            }

            return Brushes.Transparent;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                if (ts.TotalSeconds < 0)
                    return "עבר הזמן";

                var parts = new System.Collections.Generic.List<string>();

                if (ts.Days > 0)
                    parts.Add($"עוד {ts.Days} ימים");

                if (ts.Hours > 0)
                    parts.Add($"{ts.Hours} שעות");

                if (ts.Minutes > 0)
                    parts.Add($"{ts.Minutes} דקות");

                if (parts.Count == 0)
                    parts.Add("פחות מדקה");

                return string.Join(", ", parts);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

