using JolTudomE_Api.Models;
using JolTudomE_Api.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace JolTudomE_Api.Controllers {
  public class BaseController:ApiController {

    protected JolTudomEEntities DBContext { get; private set; }
    protected SessionManager SM { get; private set; }

    public BaseController() {
      DBContext = new JolTudomEEntities();
      CustomIdentity id = User.Identity as CustomIdentity;
      if (id != null) {
        SM = new SessionManager(id.Token);
      }
    }

    protected void UpdateSession() {
      SM.UpdateSessionLastAction();
    }
  }
}