using JolTudomE_Api.Models;
using JolTudomE_Api.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JolTudomE_Api.Controllers {

  [RoutePrefix("api/test")]
  [Authorize]
  public class TestController : BaseController {

    [Route("statistic/{personid:int}")]
    public IEnumerable<usp_Statistics_Result> GetStatistics(int personid) {
      var id = (CustomIdentity)User.Identity;
      IOrderedEnumerable<usp_Statistics_Result> statistics = null;
      try {
        statistics = DBContext.usp_Statistics(personid, id.PersonID, id.RoleID).OrderByDescending(s => s.Generated);
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
      return statistics;
    }

    [Route("detail/{testid}/{personid:int}")]
    public IEnumerable<usp_Eval_Result> GetTestDetails(int testid, int personid) {
      var id = (CustomIdentity)User.Identity;
      ObjectResult<usp_Eval_Result> details = null;
      try {
        details = DBContext.usp_Eval(testid, personid, id.PersonID, id.RoleID);
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

      return details;
    }

    [Route("start/{personid}/{count}/{topicids}")]
    [HttpGet]
    public NewTest StartTest(int personid, int count, string topicids) {

      // topicids needs to be parsed to List<int>
      List<int> inttopics = new List<int>();
      topicids.Split(',').ToList().ForEach(t => inttopics.Add(int.Parse(t)));

      DataTable dtTopics = new DataTable();
      dtTopics.Columns.Add("TID", typeof(int));
      foreach (int i in inttopics) {
        var r = dtTopics.NewRow();
        r[0] = i;
        dtTopics.Rows.Add(r);
      }

      SqlParameter ptopicid = new SqlParameter("@topicid", dtTopics);
      ptopicid.SqlDbType = SqlDbType.Structured;
      ptopicid.TypeName = "dbo.TopicIDs";

      SqlParameter pcount = new SqlParameter("@count", count);
      SqlParameter ppersonid = new SqlParameter("@personid", personid);

      string query = "[test].[usp_NewTest] @count, @topicid, @personid";

      string connstring = System.Configuration.ConfigurationManager.ConnectionStrings["JolTudomEEntities"].ConnectionString;
      string searchterm = ";provider connection string=\"";
      connstring = connstring.Substring(connstring.IndexOf(searchterm) + searchterm.Length);
      connstring = connstring.Substring(0, connstring.IndexOf('"'));

      var conn = new SqlConnection(connstring);

      var cmd = conn.CreateCommand();
      cmd.CommandText = query;

      cmd.Parameters.Add(pcount);
      cmd.Parameters.Add(ptopicid);
      cmd.Parameters.Add(ppersonid);

      cmd.CommandTimeout = 700;
      SqlDataAdapter dap = new SqlDataAdapter(cmd);
      DataTable dtresult = new DataTable();
      bool closed = conn.State == System.Data.ConnectionState.Closed;
      if (closed) conn.Open();
      try {
        dap.Fill(dtresult);
      }
      finally {
        if (closed) conn.Close();
      }

      List<usp_ContineTest_Result> resultlist = new List<usp_ContineTest_Result>();
      foreach (DataRow drow in dtresult.Rows) {
        resultlist.Add(new usp_ContineTest_Result {
          TestID = int.Parse(drow["TestID"].ToString()),
          PersonID = int.Parse(drow["PersonID"].ToString()),
          QuestionID = int.Parse(drow["QuestionID"].ToString()),
          QuestionText = drow["QuestionText"].ToString(),
          AnswerID = int.Parse(drow["AnswerID"].ToString()),
          AnswerText = drow["AnswerText"].ToString(),
        });
      }

      NewTest newtest = ParseNewContinueResult(resultlist);
      UpdateSession();

      return newtest;
    }

    private NewTest ParseNewContinueResult(List<usp_ContineTest_Result> result) {
      NewTest newtest = new NewTest();
      newtest.TestID = result.First().TestID;
      newtest.PersonID = result.First().PersonID;

      int questid = result.First().QuestionID;
      string questtext = result.First().QuestionText;

      List<NewTestQuestAnswer> questanswlist = new List<NewTestQuestAnswer>();

      foreach (usp_ContineTest_Result row in result) {
        if (questid != row.QuestionID) {

          var testq = new NewTestQuestion {
            QuestionID = questid,
            QuestionText = questtext,
            Answers = questanswlist
          };
          newtest.Questions.Add(testq);

          questid = row.QuestionID;
          questtext = row.QuestionText;

          questanswlist = new List<NewTestQuestAnswer>();
        }

        questanswlist.Add(new NewTestQuestAnswer {
          AnswerID = (int)row.AnswerID,
          AnswerText = row.AnswerText,
        });

      }
      newtest.Questions.Add(new NewTestQuestion {
        QuestionID = questid,
        QuestionText = questtext,
        Answers = questanswlist
      });

      return newtest;
    }

    [Route("answer/{testid}/{questionid}/{answerid}")]
    [HttpGet]
    public IHttpActionResult CheckTest(int testid, int questionid, int answerid) {
      var id = (CustomIdentity)User.Identity;
      try {
        DBContext.usp_CheckedAnswer(testid, questionid, answerid, false);
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

      return Ok();
    }

    [Route("complete/{testid}/{questionid}/{answerid}")]
    [HttpGet]
    public IHttpActionResult CompleteTest(int testid, int questionid, int answerid) {
      var id = (CustomIdentity)User.Identity;
      try {
        DBContext.usp_CheckedAnswer(testid, questionid, answerid, true);
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
      return Ok();
    }

    [Route("cancel/{testid}/{personid}")]
    [HttpGet]
    public IHttpActionResult Cancel(int testid, int personid) {
      var id = (CustomIdentity)User.Identity;
      try {
        DBContext.usp_CancelTest(testid, personid);
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

      return Ok();
    }

    [Route("suspend/{testid}")]
    [HttpGet]
    public IHttpActionResult SuspendEvent(int testid) {
      var id = (CustomIdentity)User.Identity;
      try {
        DBContext.usp_AddEvent(testid, 1);
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
      return Ok();
    }

    [Route("resume/{testid}")]
    [HttpGet]
    public IHttpActionResult ResumeEvent(int testid) {
      var id = (CustomIdentity)User.Identity;
      try {
        DBContext.usp_AddEvent(testid, 0);
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

      return Ok();
    }

    [Route("continue/{personid}")]
    [HttpGet]
    public NewTest ContinueTest(int personid) {
      NewTest conttest;
      try {
        var result = DBContext.usp_ContineTest(personid);

        conttest = ParseNewContinueResult(result.ToList());

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
      return conttest;
    }
  }
}