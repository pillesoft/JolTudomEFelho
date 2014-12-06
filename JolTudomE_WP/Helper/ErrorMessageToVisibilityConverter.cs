using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JolTudomE_WP.Helper
{
  public class ErrorMessageToVisibilityConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, string language) {
      if (string.IsNullOrEmpty(value.ToString())) {
        return Visibility.Collapsed;
      }
      else {
        return Visibility.Visible;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      string asdf = "Sdfg";
      //if (value == null || !(value is bool)) {
      //  return string.Empty;
      //}
      //if (parameter == null || !(parameter is string)) {
      //  return string.Empty;
      //}
      //if ((bool)value) {
      //  return parameter.ToString();
      //}
      //else {
      //  return string.Empty;
      //}
      return "asdf";
    }
  }
}
