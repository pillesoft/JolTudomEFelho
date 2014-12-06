using JolTudomE_WP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP.Model {
  public abstract class BaseEditable:BaseNotifyable {
    #region Error handling

    private ObservableDictionary _Errors = new ObservableDictionary();

    public ObservableDictionary Errors {
      get { return _Errors; }
    }

    public bool IsValid {
      get {
        return _Errors.Count == 0;
      }
    }

    #endregion

  }
}
