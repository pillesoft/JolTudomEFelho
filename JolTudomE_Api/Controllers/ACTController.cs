using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JolTudomE_Api.Controllers {
  // this is a controller to test if the server is ready - AccessTest
  [RoutePrefix("api/act")]
  public class ACTController : BaseController {
    [Route("ping")]
    [AllowAnonymous]
    [HttpGet]
    public string ping() {
      return "pong";
    }

    [Route("pingdb")]
    [AllowAnonymous]
    [HttpGet]
    public string pingdb() {
      int howmany = DBContext.Database.SqlQuery<int>("select count(*) from users.person").Single();
      return string.Format("{0} sor van a tablaba", howmany);
    }

  }
}
