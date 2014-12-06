using JolTudomE_WP.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace JolTudomE_WP.Helper
{
  public class TopicSelectionChangedConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, string language) {
      List<int> idlist = new List<int>();
      ListView lv = parameter as ListView;

      foreach (Topic t in lv.SelectedItems) {
        idlist.Add(t.TopicID);
      }

      return idlist;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
      throw new NotImplementedException();
    }
  }
}
