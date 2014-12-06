using JolTudomE_WP.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace JolTudomE_WP.Helper {
  public class ExceptionHandler {

    private static List<SPError> _SPErrorCodes = new List<SPError>() { 
      new SPError{SPName = "Add New User Error", ErrorCode = 1, ErrorMessage = "A megadott felhasználó már regisztrálva van"},
      new SPError{SPName = "Add New User Error", ErrorCode = 10, ErrorMessage = "A felhasználó név 5 és 8 karakter között kell hogy legyen"},
      new SPError{SPName = "Add New User Error", ErrorCode = 100, ErrorMessage = "A jelszó minimum 8 karakter kell hogy legyen"},
      new SPError{SPName = "Add New User Error", ErrorCode = 1000, ErrorMessage = "A csoport azonosító nem megfelelő"},
    };

    internal static string GetUserFriendlyErrorMessage(Dictionary<string, string> parsedmessage) {
      if (parsedmessage["ExceptionType"] == "JolTudomE_Api.Models.SPException") 
        return GetSPErrorMessage(parsedmessage["ExceptionMessage"]);
      else
        return parsedmessage["ExceptionMessage"];
    }

    /// <summary>
    /// calculates the collection of the error codes recevied from SQL stored procedure
    /// </summary>
    /// <param name="excobj"></param>
    /// <returns>new line separated error messages</returns>
    private static string GetSPErrorMessage(string sqlmessage) {
      string msg = string.Empty;

      string spname = sqlmessage.Split('.')[0];
      int code = int.Parse(sqlmessage.Split(':')[1]);
      List<int> codes = new List<int>();
      int quotbase = 1000000;

      while (true) {
        if (code >= quotbase) {
          int modulo = code % quotbase;
          if (modulo == 0) {
            codes.Add(quotbase);
            break;
          }
          else if (modulo >= 1) {
            codes.Add(quotbase);
            code = code - quotbase;
            quotbase = quotbase / 10;
          }
        }
        else
          quotbase = quotbase / 10;
      }

      StringBuilder sb = new StringBuilder();
      foreach (int err in codes) {
        sb.AppendLine(string.Format("{0} - {1}", err, _SPErrorCodes.Find(e => e.ErrorCode == err && e.SPName == spname).ErrorMessage));
      }

      msg = sb.ToString();
      return msg;
    }


  }
}
