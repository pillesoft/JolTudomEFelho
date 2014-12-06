using JolTudomE_WP.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JolTudomE_WP {
  sealed class DataSource {

    private static DataSource _DataSource = new DataSource();

    private WebAPIManager _WAM;
    private ObservableCollection<PersonRole> _Roles;
    private LoginResponse _LoggedInInfo;
    private LoginResponse _SelectedUserInfo;
    private int? _CurrentTestID;
    private NewTestParam _NewTestParam;

    public DataSource() {
      _WAM = new WebAPIManager();

      _Roles = new ObservableCollection<PersonRole>();
      _Roles.Add(new PersonRole { RoleID = 1, Role = "Diák", GroupingOrder = 15 });
      _Roles.Add(new PersonRole { RoleID = 2, Role = "Tanár", GroupingOrder = 10 });
      _Roles.Add(new PersonRole { RoleID = 3, Role = "Admin", GroupingOrder = 5 });
      _Roles.Add(new PersonRole { RoleID = 999, Role = "Saját", GroupingOrder = 0 });
    }


    internal static LoginResponse LoggedInInfo {
      get { return _DataSource._LoggedInInfo; }
      set { _DataSource._LoggedInInfo = value; }
    }

    internal static LoginResponse SelectedUserInfo {
      get { return _DataSource._SelectedUserInfo; }
      set { _DataSource._SelectedUserInfo = value; }
    }

    internal static int? CurrentTest {
      get { return _DataSource._CurrentTestID; }
      set { _DataSource._CurrentTestID = value; }
    }

    internal static bool HasCurrentTest {
      get {
        return _DataSource._CurrentTestID != null;
      }
    }

    internal static void AddNewTestParam(NewTestParam param) {
      _DataSource._NewTestParam = param;
    }

    internal static ObservableCollection<PersonRole> GetRoles() {
      return _DataSource._Roles; 
    }

    internal static PersonRole GetRoleByName(string name) {
      return GetRoles().FirstOrDefault(r => r.Role == name);
    }
    internal static PersonRole GetRoleById(int id) {
      return GetRoles().FirstOrDefault(r => r.RoleID == id);
    }
    internal static PersonRole GetRoleStudent() {
      return GetRoleById(1);
    }

    internal static UserDetail CreateUserStudent() {
      return new UserDetail { UserName = "", Prefix="", MiddleName="", Password = "", FirstName = "", LastName = "", Role = GetRoleByName("Diák") };
    }

    internal static bool IsAuthenticated {
      get { return _DataSource._LoggedInInfo != null; }
    }

    internal async static Task MakeLogin(string username, string password, Action postlogin) {
      var loginresult = await _DataSource._WAM.Login(username, password);

      _DataSource._LoggedInInfo = JsonConvert.DeserializeObject<LoginResponse>(loginresult);

      ((App)App.Current).SaveCredential(username, password);

      if (DataSource.LoggedInInfo.RoleID == DataSource.GetRoleStudent().RoleID) {
        DataSource.SelectedUserInfo = DataSource.LoggedInInfo;
      }

      postlogin();

    }

    internal async static Task<bool> MakeRegister(UserDetail newuser) {
      await _DataSource._WAM.Register(newuser);

      return true;
    }

    internal async static Task<List<GroupedUser>> GetUserList() {
      if (!IsAuthenticated) return null;

      string result = string.Empty;
      if (_DataSource._LoggedInInfo.RoleID == 2) {
        // teacher
        // fetch students only
        result = await _DataSource._WAM.GetUserList(GetRoleStudent().RoleID);
      }
      if (_DataSource._LoggedInInfo.RoleID == 3) {
        // admin
        // fetch everybody
        result = await _DataSource._WAM.GetUserList(null);
      }
      if (result == null) return null;

      var ul = JsonConvert.DeserializeObject<List<UserDetail>>(result);
      List<GroupedUser> userlist = new List<GroupedUser>();

      // convert it to GroupedList
      if (_DataSource._LoggedInInfo.RoleID == 2) {
        userlist.Add(new GroupedUser { DisplayName = _DataSource._LoggedInInfo.DisplayName, Role = GetRoleById(999), PersonID = _DataSource._LoggedInInfo.PersonID });
        foreach (UserDetail ud in ul) {
          userlist.Add(new GroupedUser { DisplayName = string.Format("{0}, {1}", ud.LastName.ToUpper(), ud.FirstName), Role = GetRoleById(1), PersonID = ud.PersonID });
        }
      }
      else if (_DataSource._LoggedInInfo.RoleID == 3) {
        // remove the logged in user
        ul.Remove(ul.Find(u => u.PersonID == _DataSource._LoggedInInfo.PersonID));
        // add the logged in user
        userlist.Add(new GroupedUser { DisplayName = _DataSource._LoggedInInfo.DisplayName, Role = GetRoleById(999), PersonID = _DataSource._LoggedInInfo.PersonID });
        foreach (UserDetail ud in ul) {
          userlist.Add(new GroupedUser { DisplayName = string.Format("{0}, {1}", ud.LastName.ToUpper(), ud.FirstName), Role = GetRoleById(ud.RoleID), PersonID = ud.PersonID });
        }
      }

      return userlist;
    }

    internal async static Task<UserDetail> GetLoginDetail() {
      if (!IsAuthenticated) return null;
      var result = await _DataSource._WAM.GetLoginDetail();
      if (result != null) {
        
        // intentionally make delay to see if the progress bar is working
        //await Task.Delay(TimeSpan.FromSeconds(20));

        var userdet = JsonConvert.DeserializeObject<UserDetail>(result);
        return userdet;
      }
      else return null;
    }

    internal async static Task<List<Statistic>> GetStatistic() {
      if (!IsAuthenticated) return null;
      var result = await _DataSource._WAM.GetStatistics(_DataSource._SelectedUserInfo.PersonID);
      if (result != null) {
        var statlist = JsonConvert.DeserializeObject<List<Statistic>>(result);
        return statlist;
      }
      else return null;
    }

    internal async static Task<List<TestDetail>> GetTestDetail(int testid) {
      if (!IsAuthenticated) return null;

      var result = await _DataSource._WAM.GetTestDetails(testid, _DataSource._SelectedUserInfo.PersonID);
      if (result != null) {
        var testdetlist = JsonConvert.DeserializeObject<List<TestDetail>>(result);
        return testdetlist;
      }
      else return null;
    }

    internal async static Task<ObservableCollection<Course>> GetCourses() {
      if (!IsAuthenticated) return null;
      var result = await _DataSource._WAM.GetCourses();
      if (result != null) {
        var courselist = JsonConvert.DeserializeObject<ObservableCollection<Course>>(result);
        return courselist;
      }
      else return null;
    }

    internal async static Task<ObservableCollection<Topic>> GetTopics(int courseid) {
      if (!IsAuthenticated) return null;
      var result = await _DataSource._WAM.GetTopics(courseid);
      if (result == null) return null;

      var topiclist = JsonConvert.DeserializeObject<ObservableCollection<Topic>>(result);
      return topiclist;
    }

    internal async static Task<NewTest> GenerateTest() {
      if (!IsAuthenticated) return null;
      var result = await _DataSource._WAM.StartNewTest(_DataSource._LoggedInInfo.PersonID, _DataSource._NewTestParam.NumberOfQuestions, _DataSource._NewTestParam.TopicIDs);
      if (result == null) return null;

      var newtest = JsonConvert.DeserializeObject<NewTest>(result);
      _DataSource._CurrentTestID = newtest.TestID;

      return newtest;
    }

    internal async static Task AnswerTest(int testid, int questionid, int answerid) {
      if (IsAuthenticated)
        await _DataSource._WAM.AnswerTest(testid, questionid, answerid);
    }

    internal async static Task CompleteTest(int testid, int questionid, int answerid) {
      if (IsAuthenticated) {
        await _DataSource._WAM.CompleteTest(testid, questionid, answerid);
        _DataSource._CurrentTestID = null;
      }
    }

    internal async static Task CancelTest(int testid) {
      if (IsAuthenticated) {
        await _DataSource._WAM.CancelTest(testid, _DataSource._LoggedInInfo.PersonID);
        _DataSource._CurrentTestID = null;
      }
    }

    internal async static Task SuspendTest() {
      if (IsAuthenticated) {
        await _DataSource._WAM.SuspendTest((int)_DataSource._CurrentTestID);
      }
    }

    internal async static Task ResumeTest() {
      if (IsAuthenticated) {
        await _DataSource._WAM.ResumeTest((int)_DataSource._CurrentTestID);
      }
    }

    internal async static Task<NewTest> ContinueTest() {
      if (!IsAuthenticated) return null;
      var result = await _DataSource._WAM.ContinueTest(_DataSource._LoggedInInfo.PersonID);
      if (result == null) return null;

      var newtest = JsonConvert.DeserializeObject<NewTest>(result);
      _DataSource._CurrentTestID = newtest.TestID;

      return newtest;
    }

  }
}
