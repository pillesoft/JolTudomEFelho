using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP.Model {
  public abstract class BaseNotifyable : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string caller) {
      var handler = PropertyChanged;
      if (handler != null) {
        handler(this, new PropertyChangedEventArgs(caller));
      }
    }

    protected bool SetProperty<T>(ref T prop, T value, [CallerMemberName]string caller = null) {
      if (EqualityComparer<T>.Default.Equals(prop, value)) return false;

      prop = value;
      OnPropertyChanged(caller);
      return true;
    }
  }

}

