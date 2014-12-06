using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace JolTudomE_Api.Security {
  public class CustomPrincipal:IPrincipal {

    private CustomIdentity _Identity;

    public CustomPrincipal(CustomIdentity identity) {
      _Identity = identity;
    }

    public IIdentity Identity {
      get { return _Identity; }
    }

    public bool IsInRole(string role) {
      return true;
    }
  }
}