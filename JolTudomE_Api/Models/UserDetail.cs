using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JolTudomE_Api.Models {

  public class UserDetail {

    #region properties

    private int _PersonID;

    public int PersonID {
      get { return _PersonID; }
      set { _PersonID = value; }
    }

    private string _UserName;
    [Required(AllowEmptyStrings=false, ErrorMessage="Ki kell tölteni a Felhasználó nevet")]
    [StringLength(8, MinimumLength = 5, ErrorMessage = "Felhasználó neve 5 és 8 karakter között kell legyen")]
    public string UserName {
      get { return _UserName; }
      set { _UserName = value; }
    }

    private string _Prefix;
    [StringLength(5, ErrorMessage="Maximum 5 karakter lehet az Előtag")]
    public string Prefix {
      get { return _Prefix; }
      set {_Prefix = value;
      }
    }

    private string _FirstName;
    [Required(AllowEmptyStrings = false, ErrorMessage = "Ki kell tölteni a Keresztnevet")]
    public string FirstName {
      get { return _FirstName; }
      set {_FirstName = value;
      }
    }

    private string _MiddleName;
    public string MiddleName {
      get { return _MiddleName; }
      set {_MiddleName = value;
      }
    }

    private string _LastName;
    [Required(AllowEmptyStrings=false, ErrorMessage="Ki kell tölteni a Család nevet")]
    public string LastName {
      get { return _LastName; }
      set {
        _LastName = value;
      }
    }
 
    private string _Password;
    [Required(AllowEmptyStrings=false, ErrorMessage="Ki kell tölteni a Jelszót")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Jelszó minimum 8 karakter kell legyen")]
    public string Password {
      get { return _Password; }
      set { _Password = value; }
    }

    private int _RoleID;

    [Range(1, 3, ErrorMessage = "Csoport azonosító 1 és 3 között kell legyen")]
    public int RoleID {
      get { return _RoleID; }
      set { _RoleID = value; }
    }


    #endregion

  }
}