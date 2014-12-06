using JolTudomE_Api.Models;
using JolTudomE_Api.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace JolTudomE_Api.Controllers {
  [RoutePrefix("api/account")]
  [Authorize]
  public class AccountController : BaseController {

    [Route("login")]
    [AllowAnonymous]
    public LoginResponse Login() {
      var id = (CustomIdentity)User.Identity;

      var p = DBContext.Person.Find(id.PersonID);
      return new LoginResponse { PersonID = id.PersonID, RoleID = id.RoleID, DisplayName = string.Format("{0}, {1}", p.LastName.ToUpper(), p.FirstName) };
    }

    [Route("detail")]
    [HttpGet]
    public UserDetail GetUserInfo() {
      var id = (CustomIdentity)User.Identity;

      var p = DBContext.Person.Find(id.PersonID);
      var newperson = new UserDetail {
        FirstName = p.FirstName,
        LastName = p.LastName,
        MiddleName = p.MiddleName,
        PersonID = p.PersonID,
        Prefix = p.Prefix,
        UserName = p.UserName, 
        RoleID = id.RoleID,
        Password = string.Empty
      };
      UpdateSession();

      return newperson;
    }

    [Route("searchbyrole/{roleid:int?}")]
    [HttpGet]
    public IEnumerable<usp_GetAllUsers_Result> GetUsers(int? roleid = null) {
      var id = (CustomIdentity)User.Identity;
      ObjectResult<usp_GetAllUsers_Result> users = null;
      try {
        users = DBContext.usp_GetAllUsers(id.RoleID, roleid);
        UpdateSession();
      }
      catch (EntityCommandExecutionException ece_exc) {
        if (ece_exc.InnerException.GetType() == typeof(SqlException)) {
          SqlException sqlexc = (SqlException)ece_exc.InnerException;
          if (sqlexc.Number == 50000) throw new SPException(sqlexc.Message);
          else throw new DBException(sqlexc.Message);
        }
        else {
          throw ece_exc.InnerException;
        }
      }
      return users;
    }

    [Route("addstudent")]
    [HttpPost]
    [AllowAnonymous]
    public HttpResponseMessage AddStudent(UserDetail student) {
      if (this.ModelState.IsValid) {
        try {
          // roleid = 1 ---> student
          DBContext.usp_AddNewUser(student.UserName, student.Prefix, student.LastName, student.MiddleName, student.FirstName, student.Password, 1);
          var response = Request.CreateResponse(HttpStatusCode.Created);
          return response;
        }
        catch (EntityCommandExecutionException ece_exc) {
          if (ece_exc.InnerException.GetType() == typeof(SqlException)) {
            SqlException sqlexc = (SqlException)ece_exc.InnerException;
            if (sqlexc.Number == 50000) throw new SPException(sqlexc.Message);
            else throw new DBException(sqlexc.Message);
          }
          else {
            throw ece_exc.InnerException;
          }
        }
      }
      else {
        return Request.CreateResponse(HttpStatusCode.BadRequest, this.ModelState);
      }
    }

  }
}
