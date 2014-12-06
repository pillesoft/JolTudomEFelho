using JolTudomE_WP.Helper;
using JolTudomE_WP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP {

  class UnauthorizedException : Exception { }
  
  class WebApiException : Exception {
    public WebApiException(string apierror) : base(apierror) { }
  }

  class ApiModelError : Dictionary<string, List<string>> {
    public string FormattedError {
      get {
        StringBuilder err = new StringBuilder();
        foreach (var item in this) {
          err.AppendLine(string.Format("Field Name: {0}", item.Key));
          foreach (var value in item.Value) {
            err.AppendLine(string.Format("--->{0}", value));
          }
        }
        return err.ToString();
      }
    }
  }

  class WebAPIManager {
    private string _Token;

    private const string WEBAPIROOT = "http://localhost:1854";

    private string GetResponse(HttpWebResponse webresp) {
      Stream dataStream = webresp.GetResponseStream();
      using (StreamReader reader = new StreamReader(dataStream)) {
        string responseFromServer = reader.ReadToEnd();
        return responseFromServer;
      }

    }

    private async Task<string> DoRequest(string url) {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.Method = "GET";
      request.Accept = "application/json";

      Cookie c = new Cookie("JolTudomEToken", _Token);
      request.CookieContainer = new CookieContainer();
      request.CookieContainer.Add(new Uri(WEBAPIROOT), c);

      string responseFromServer = string.Empty;

      try {
        WebResponse response = await request.GetResponseAsync();
        HttpWebResponse httpresp = (HttpWebResponse)response;

        if (httpresp.StatusCode == HttpStatusCode.OK) {
          responseFromServer = GetResponse(httpresp);
        }
        return responseFromServer;
      }
      catch (WebException wexc) {
        if (((HttpWebResponse)wexc.Response).StatusCode == HttpStatusCode.Unauthorized) {
          // that means the session is expired
          _Token = string.Empty;
          DataSource.LoggedInInfo = null;
          ((App)App.Current).SessionExpired();
          return null;
        }
        else if (((HttpWebResponse)wexc.Response).StatusCode == HttpStatusCode.InternalServerError) {
          string modelerrors = GetResponse((HttpWebResponse)wexc.Response);
          Dictionary<string, string> errdetails = new Dictionary<string,string>();
          try {
            errdetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(modelerrors);
          }
          catch {
          // write the message to the Log, because maybe the deserialization will be failed
            App.LogIt.LogError(string.Format("Response Internal Server Deserialization Error-{0}\n{1}", url, modelerrors));
            ((App)App.Current).ShowDialog("Jól Tudom E - Hiba", "Nem várt hiba történt! Hívja a fejlesztőt!");
          }
          string errmess = ExceptionHandler.GetUserFriendlyErrorMessage(errdetails);
          App.LogIt.LogError(string.Format("Response DB Error-{0}\n{1}", url, errmess));
          ((App)App.Current).ShowDialog("Adatbázis Hiba", wexc.Message);
          return null;
        }
        else {
          App.LogIt.LogError(string.Format("Response Unhandled Error-{0}\n{1}", url, wexc.ToString()));
          ((App)App.Current).ShowDialog("Nem várt Hiba", wexc.Message);
          return null;
        }
      }

    }

    internal async Task<string> Login(string username, string password) {
      string fullurl = string.Format("{0}/{1}", WEBAPIROOT, "api/account/login");
      App.LogIt.LogInfo(string.Format("Login-{0}-{1}", fullurl, username));

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullurl);

      request.Method = "POST";
      string basicauth = username + ":" + password;
      basicauth = Convert.ToBase64String(Encoding.UTF8.GetBytes(basicauth));
      request.Headers["Authorization"] = "Basic " + basicauth;

      try {
        WebResponse response = await request.GetResponseAsync();
        HttpWebResponse httpresp = (HttpWebResponse)response;
        string responseFromServer = string.Empty;

        if (httpresp.StatusCode == HttpStatusCode.OK) {
          string cookie = httpresp.Headers["Set-Cookie"];
          _Token = cookie.Split('=')[1];

          responseFromServer = GetResponse(httpresp);
          response.Dispose();

        }
        else {
          _Token = string.Empty;
        }

        return responseFromServer;
      }
      catch (WebException wexc) {
        _Token = string.Empty;
        if (((HttpWebResponse)wexc.Response).StatusCode == HttpStatusCode.Unauthorized) throw new UnauthorizedException();
        else throw;
      }
    }

    internal async Task<bool> Register(UserDetail newuser) {
      string fullurl = string.Format("{0}/{1}", WEBAPIROOT, "api/account/addstudent");
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullurl);

      request.Method = "POST";
      request.ContentType = "application/json; charset=utf-8";
      request.Accept = "application/json";

      string postData = JsonConvert.SerializeObject(newuser);
      byte[] byteArray = Encoding.UTF8.GetBytes(postData);
      Stream dataStream = await request.GetRequestStreamAsync();
      dataStream.Write(byteArray, 0, byteArray.Length);
      dataStream.Dispose();

      // send it
      try {
        WebResponse response = await request.GetResponseAsync();
        HttpWebResponse httpresp = (HttpWebResponse)response;
        string responseFromServer = string.Empty;

        if (httpresp.StatusCode == HttpStatusCode.Created) {
          response.Dispose();
        }

        return true;
      }
      catch (WebException wexc) {
        if (((HttpWebResponse)wexc.Response).StatusCode == HttpStatusCode.BadRequest) {
          string modelerrors = GetResponse((HttpWebResponse)wexc.Response);
          var errdetails = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>>(modelerrors);
          ApiModelError apierr = new ApiModelError();
          foreach (var field in errdetails) {
            var errlist = field.Value["_errors"].FindAll(e => e.ContainsKey("<ErrorMessage>k__BackingField"));
            List<string> errmesslist = new List<string>();
            foreach (var errdet in errlist) {
              string errmsg = string.Empty;
              if (errdet.TryGetValue("<ErrorMessage>k__BackingField", out errmsg)) errmesslist.Add(errmsg);
            }
            apierr.Add(field.Key, errmesslist);
          }
          throw new WebApiException(apierr.FormattedError);
        }
        else if (((HttpWebResponse)wexc.Response).StatusCode == HttpStatusCode.InternalServerError) {
          string modelerrors = GetResponse((HttpWebResponse)wexc.Response);
          Dictionary<string, string> errdetails = new Dictionary<string, string>();
          try {
            errdetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(modelerrors);
          }
          catch {
            // write the message to the Log, because maybe the deserialization will be failed
            App.LogIt.LogError(string.Format("Register Internal Server Deserialization Error\n{0}", modelerrors));
            ((App)App.Current).ShowDialog("Jól Tudom E - Hiba", "Nem várt hiba történt! Hívja a fejlesztőt!");
          }
          string errmess = ExceptionHandler.GetUserFriendlyErrorMessage(errdetails);
          App.LogIt.LogError(string.Format("Register DB Error\n{0}", errmess));
          throw new WebApiException(errmess);
        }
        else throw;
      }

    }

    internal async Task<string> GetLoginDetail() {
      string fullurl = string.Format("{0}/{1}", WEBAPIROOT, "api/account/detail");
      App.LogIt.LogInfo(string.Format("GetLoginDetail-{0}", fullurl));
      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }

    internal async Task<string> GetUserList(int? roletosearch) {
      string fullurl = string.Empty;
      if (roletosearch == null) {
        fullurl = string.Format("{0}/{1}", WEBAPIROOT, "api/account/searchbyrole");
      }
      else {
        fullurl = string.Format("{0}/{1}/{2}", WEBAPIROOT, "api/account/searchbyrole", roletosearch);
      }
      App.LogIt.LogInfo(string.Format("GetUserList-{0}", fullurl));
      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }

    internal async Task<string> GetStatistics(int personid) {
      string fullurl = string.Format("{0}/{1}/{2}", WEBAPIROOT, "api/test/statistic", personid);
      App.LogIt.LogInfo(string.Format("GetStatistics-{0}", fullurl));

      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }

    internal async Task<string> GetTestDetails(int testid, int personid) {
      string fullurl = string.Format("{0}/{1}/{2}/{3}", WEBAPIROOT, "api/test/detail", testid, personid);
      App.LogIt.LogInfo(string.Format("GetTestDetails-{0}", fullurl));

      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }

    internal async Task<string> GetCourses() {
      string fullurl = string.Format("{0}/{1}", WEBAPIROOT, "api/course/courses");
      App.LogIt.LogInfo(string.Format("GetCourses-{0}", fullurl));

      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }

    internal async Task<string> GetTopics(int courseid) {
      string fullurl = string.Format("{0}/{1}/{2}", WEBAPIROOT, "api/course/topic", courseid);
      App.LogIt.LogInfo(string.Format("GetTopics-{0}", fullurl));

      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }

    internal async Task<string> StartNewTest(int personid, int count, List<int> topicids) {
      string fullurl = string.Format("{0}/{1}/{2}/{3}/{4}", WEBAPIROOT, "api/test/start", personid, count, string.Join(",", topicids));
      App.LogIt.LogInfo(string.Format("StartNewTest-{0}", fullurl));

      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }

    internal async Task AnswerTest(int testid, int questionid, int answerid) {
      string fullurl = string.Format("{0}/{1}/{2}/{3}/{4}", WEBAPIROOT, "api/test/answer", testid, questionid, answerid);
      App.LogIt.LogInfo(string.Format("AnswerTest-{0}", fullurl));

      try {
        await DoRequest(fullurl);
      }
      catch {
        throw;
      }
    }

    internal async Task CompleteTest(int testid, int questionid, int answerid) {
      string fullurl = string.Format("{0}/{1}/{2}/{3}/{4}", WEBAPIROOT, "api/test/complete", testid, questionid, answerid);
      App.LogIt.LogInfo(string.Format("CompleteTest-{0}", fullurl));

      try {
        await DoRequest(fullurl);
      }
      catch {
        throw;
      }
    }

    internal async Task CancelTest(int testid, int personid) {
      string fullurl = string.Format("{0}/{1}/{2}/{3}", WEBAPIROOT, "api/test/cancel", testid, personid);
      App.LogIt.LogInfo(string.Format("CancelTest-{0}", fullurl));

      try {
        await DoRequest(fullurl);
      }
      catch {
        throw;
      }

    }

    internal async Task SuspendTest(int testid) {
      string fullurl = string.Format("{0}/{1}/{2}", WEBAPIROOT, "api/test/suspend", testid);
      App.LogIt.LogInfo(string.Format("SuspendTest-{0}", fullurl));

      try {
        await DoRequest(fullurl);
      }
      catch {
        throw;
      }
    }

    internal async Task ResumeTest(int testid) {
      string fullurl = string.Format("{0}/{1}/{2}", WEBAPIROOT, "api/test/resume", testid);
      App.LogIt.LogInfo(string.Format("ResumeTest-{0}", fullurl));

      try {
        await DoRequest(fullurl);
      }
      catch {
        throw;
      }
    }

    internal async Task<string> ContinueTest(int personid) {
      string fullurl = string.Format("{0}/{1}/{2}", WEBAPIROOT, "api/test/continue", personid);
      App.LogIt.LogInfo(string.Format("ContinueTest-{0}", fullurl));

      string response = string.Empty;
      try {
        response = await DoRequest(fullurl);
      }
      catch {
        throw;
      }
      return response;
    }
  }
}
