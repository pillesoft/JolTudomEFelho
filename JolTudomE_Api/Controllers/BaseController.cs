using JolTudomE_Api.Models;
using JolTudomE_Api.Security;
using JolTudomE_Api.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace JolTudomE_Api.Controllers {
  public class BaseController : ApiController {

    protected JolTudomEEntities DBContext { get; private set; }
    protected SessionManager SM { get; private set; }
    protected StorageManager StorMan { get; private set; }
    protected List<PersonRole> Roles { get; private set; }

    public BaseController() {
      DBContext = new JolTudomEEntities();
      CustomIdentity id = User.Identity as CustomIdentity;
      if (id != null) {
        SM = new SessionManager(id.Token);
      }
      StorMan = new StorageManager();
      StorMan.Init();

      Roles = new List<PersonRole>(){
        new PersonRole { RoleID = 1, Role = "Diák" },
        new PersonRole { RoleID = 2, Role = "Tanár" },
        new PersonRole { RoleID = 3, Role = "Admin" },
      };

    }

    protected void UpdateSession() {
      SM.UpdateSessionLastAction();
    }
  }
}