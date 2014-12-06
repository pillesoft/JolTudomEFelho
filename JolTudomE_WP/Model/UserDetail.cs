using JolTudomE_WP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP.Model {

  public class UserDetail : BaseEditable {
    #region properties

    private int _PersonID;
    public int PersonID {
      get { return _PersonID; }
      set { SetProperty<int>(ref _PersonID, value); }
    }

    private string _UserName;
    public string UserName {
      get { return _UserName; }
      set {
        SetProperty<string>(ref _UserName, value);
        if (_UserName.Length < 5 || _UserName.Length > 8) {
          if (!Errors.ContainsKey("UserName")) {
            Errors.Add("UserName", "Felhasználó neve 5 és 8 karakter között kell legyen");
          }
        }
        else {
          Errors.Remove("UserName");
        }
      }
    }

    private string _Prefix;
    public string Prefix {
      get { return _Prefix; }
      set {
        SetProperty<string>(ref _Prefix, value);
        if (_Prefix != null) {
          if (_Prefix.Length > 5) {
            if (!Errors.ContainsKey("Prefix")) {
              Errors.Add("Prefix", "Maximum 5 karakter lehet az Előtag");
            }
          }
          else {
            Errors.Remove("Prefix");
          }
        }
      }
    }

    private string _FirstName;
    public string FirstName {
      get { return _FirstName; }
      set {
        SetProperty<string>(ref _FirstName, value);
        if (string.IsNullOrEmpty(_FirstName)) {
          if (!Errors.ContainsKey("FirstName")) {
            Errors.Add("FirstName", "Ki kell tölteni a Keresztnevet");
          }
        }
        else {
          Errors.Remove("FirstName");
        }
      }
    }

    private string _MiddleName;
    public string MiddleName {
      get { return _MiddleName; }
      set {
        SetProperty<string>(ref _MiddleName, value);
      }
    }

    private string _LastName;
    public string LastName {
      get { return _LastName; }
      set {
        SetProperty<string>(ref _LastName, value);
        if (string.IsNullOrEmpty(_LastName)) {
          if (!Errors.ContainsKey("LastName")) {
            Errors.Add("LastName", "Ki kell tölteni a Család nevet");
          }
        }
        else {
          Errors.Remove("LastName");
        }

      }
    }

    private string _Password;
    public string Password {
      get { return _Password; }
      set {
        SetProperty<string>(ref _Password, value);
        if (_Password.Length < 8) {
          if (!Errors.ContainsKey("Password")) {
            Errors.Add("Password", "Jelszó minimum 8 karakter kell legyen");
          }
        }
        else {
          Errors.Remove("Password");
        }
      }
    }

    private PersonRole _Role;
    public PersonRole Role {
      get { return _Role; }
      set { SetProperty<PersonRole>(ref _Role, value); }
    }

    private int _RoleID;
    public int RoleID {
      get {
        return Role.RoleID;
      }
      set {
        SetProperty<int>(ref _RoleID, value);
        Role = DataSource.GetRoleById(_RoleID);
      }
    }

    #endregion

  }
}

