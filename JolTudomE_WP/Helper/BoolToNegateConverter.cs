using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JolTudomE_WP.Helper
{
  public class BoolToNegateConverter : IValueConverter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter">if parameter is not null returns the negate of the value</param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, string language) {
      if (parameter == null) return (bool)value;
      else return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
