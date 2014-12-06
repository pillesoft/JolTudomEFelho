using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace JolTudomE_Api.Security {
  public class CustomIdentity:IIdentity {

    private string _Name;
    private string _AuthType;
    public int PersonID { get; private set; }
    public int RoleID { get; private set; }
    public string Token { get; private set; }

    public CustomIdentity(string username, string type, int personid, int roleid, string token) {
      _Name = username;
      _AuthType = type;
      PersonID = personid;
      RoleID = roleid;
      Token = token;
    }

    public string AuthenticationType {
      get { return _AuthType; }
    }

    public bool IsAuthenticated {
      get { return !string.IsNullOrEmpty(_Name); }
    }

    public string Name {
      get { return _Name; }
    }
  }
}