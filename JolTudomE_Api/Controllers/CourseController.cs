using JolTudomE_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JolTudomE_Api.Controllers {
  
  [Authorize]
  [RoutePrefix("api/course")]
  public class CourseController : BaseController {
    
    [Route("courses")]
    public IEnumerable<usp_GetCourses_Result> GetCourses() {
      ObjectResult<usp_GetCourses_Result> courses = null;
      try {
        courses = DBContext.usp_GetCourses();
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
      return courses;
    }

    [Route("topic/{courseid}")]
    public IEnumerable<usp_GetTopics_Result> GetTopics(int courseid) {
      ObjectResult<usp_GetTopics_Result> topics = null;
      try {
        topics = DBContext.usp_GetTopics(courseid);
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
      return topics;
    }

  }
}