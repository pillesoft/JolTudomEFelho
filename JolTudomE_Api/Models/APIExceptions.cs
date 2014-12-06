using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JolTudomE_Api.Models {
  [Serializable]
  public class DBException:Exception {
    public DBException(string msg):base(msg) { }
  }

  [Serializable]
  public class SPException : Exception {
    public SPException(string msg) : base(msg) { }
  }

}