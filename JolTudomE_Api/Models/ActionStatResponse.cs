using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JolTudomE_Api.Models {

  public class ActionStatResponseData{
    public string Action { get; set; }
    public string When { get; set; }
    public int HowMany { get; set; }
  }

  public class ActionStatResponse {
    public List<string> Labels { get; set; }
    public List<string> Actions { get; set; }
    public List<ActionStatResponseData> Data { get; set; }

  }
}