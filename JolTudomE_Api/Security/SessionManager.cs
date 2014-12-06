using JolTudomE_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JolTudomE_Api.Security {

  public enum JolTudomERoles {
    Student = 1,
    Teacher = 2,
    Admin = 3
  }

  /// <summary>
  /// class to manage special exception, when there is any issue with a token
  /// this thrown when the session is expired, 
  /// if the session is not found - could be that it is over of the timeout, so removed
  /// </summary>
  [Serializable]
  public class SessionNotAvailable : ApplicationException { }

  public class SessionManager {
    private Sessions _Session;
    private string _Token;

    public Sessions Session {
      get { return _Session; }
    }

    public SessionManager(string token) {
      _Token = token;
      ValidateToken();
    }

    private void GetSession() {
      using (JolTudomEEntities ent = new JolTudomEEntities()) {
        _Session = ent.Sessions.Include("Person").FirstOrDefault(s => s.Token == _Token);
        if (_Session == null) {
          throw new SessionNotAvailable();
        }
      }
    }

    private void ValidateToken() {
      GetSession();
      if (_Session.LastAction.AddMinutes(JolTudomE_Api.Properties.Settings.Default.SessionTimeoutMinute) < DateTime.UtcNow) {
        throw new SessionNotAvailable();
      }
    }

    public void UpdateSessionLastAction() {
      using (JolTudomEEntities ent = new JolTudomEEntities()) {
        //ent.Attach(_Session);
        _Session.LastAction = DateTime.UtcNow;
        ent.Entry(_Session).State = System.Data.Entity.EntityState.Modified;
        ent.SaveChanges();
      }
    }

    public void DeleteSession() {
      using (JolTudomEEntities ent = new JolTudomEEntities()) {
        //ent.Attach(_Session);
        ent.Sessions.Remove(_Session);
        ent.Entry(_Session).State = System.Data.Entity.EntityState.Deleted;
        ent.SaveChanges();
      }
    }

    public static SessionManager NewSession(int personid, int roleid) {
      // generate a token
      // this could be more secure ...
      byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
      byte[] key = Guid.NewGuid().ToByteArray();
      string token = Convert.ToBase64String(time.Concat(key).ToArray());

      using (JolTudomEEntities ent = new JolTudomEEntities()) {
        // delete those sessions, which are dead - over of the given timeout
        ent.usp_SessionsCleanup(JolTudomE_Api.Properties.Settings.Default.SessionTimeoutMinute);

        // delete those tests, which are not completed
        ent.usp_CleanupTests(JolTudomE_Api.Properties.Settings.Default.MaxTestExecutionHour);

        // this must be saved to the database with the timestamp
        ent.Sessions.Add(new Sessions { Token = token, PersonID = personid, RoleID = roleid, LastAction = DateTime.UtcNow });
        ent.SaveChanges();
      }

      SessionManager sm = new SessionManager(token);
      return sm;
    }

  }
}