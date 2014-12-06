using System;
using Windows.UI.Xaml.Data;

namespace JolTudomE_WP.Helper
{
  public class BoolToTestDetImageSourceConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language) {
      bool boolval = (bool)value;
      if (boolval) return "ms-appx:///Assets/Good.png";
      else return "ms-appx:///Assets/NotGood.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
