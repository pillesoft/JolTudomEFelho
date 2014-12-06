using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP.Model {
  public class LoginResponse {
    public string UserName { get; set; }
    public int PersonID { get; set; }
    public int RoleID { get; set; }
    public string DisplayName { get; set; }
  }
}
