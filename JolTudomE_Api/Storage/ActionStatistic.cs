using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JolTudomE_Api.Storage {
  public class ActionStatistic : TableEntity {

    public DateTime Mikor { get; set; }
    public string Role { get; set; }

    public ActionStatistic() {

    }
    public ActionStatistic(string action, string role) {
      PartitionKey = action;
      RowKey = Guid.NewGuid().ToString();
      Mikor = DateTime.Now;
      Role = role;
    }

  }
}