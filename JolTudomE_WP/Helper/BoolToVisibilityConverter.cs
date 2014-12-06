using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JolTudomE_WP.Helper
{
  public class BoolToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter">if parameter is not null Collapse returns in case of True</param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, string language) {
      if (parameter == null) return (bool)value ? Visibility.Visible : Visibility.Collapsed;
      else return (bool)value ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
