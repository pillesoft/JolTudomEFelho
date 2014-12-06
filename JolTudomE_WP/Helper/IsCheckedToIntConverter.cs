using System;
using Windows.UI.Xaml.Data;

namespace JolTudomE_WP.Helper
{
  public class IsCheckedToIntConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, string language) {
      int input = int.Parse(value.ToString());
      int param = int.Parse(parameter.ToString());
      return input == param;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      if (value == null || !(value is bool)) {
        return string.Empty;
      }
      if (parameter == null || !(parameter is string)) {
        return string.Empty;
      }
      if ((bool)value) {
        return parameter.ToString();
      }
      if (!(bool)value) {
        return 0.ToString();
      }

      else {
        return string.Empty;
      }
    }
  }
}
