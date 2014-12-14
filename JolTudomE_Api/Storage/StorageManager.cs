using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JolTudomE_Api.Storage {

  public enum TStorageType {
    ActionStatistic
  }

  public class StorageManager {

    CloudTableClient _TClient;

    public void Init() {
      CloudStorageAccount csAccount;
      
      //string connstring = "LocalStorageEmulator";
      string connstring = "AzureStorageAccount";
      var conn = ConfigurationManager.ConnectionStrings[connstring];
      if (conn == null) {
        throw new ApplicationException(string.Format("Cannot find ConnectionString '{0}'", connstring));
      }

      if (CloudStorageAccount.TryParse(conn.ConnectionString, out csAccount)) {
        _TClient = csAccount.CreateCloudTableClient();
      }
      else {
        throw new Exception("Cannot connect to Storage Account");
      }
    }

    private CloudTable GetTable(TStorageType type) {
      CloudTable ctable = _TClient.GetTableReference(type.ToString());
      ctable.CreateIfNotExists();

      return ctable;

    }

    public void InsertToTable(TStorageType type, TableEntity ent) {
      CloudTable t = GetTable(type);
      t.Execute(TableOperation.Insert(ent));
    }

    public List<T> GetData<T>(TStorageType type) where T : TableEntity, new() {
      CloudTable t = GetTable(type);
      TableQuery<T> q = new TableQuery<T>();

      return t.ExecuteQuery(q).Select(ent=> (T)ent).ToList();

    }
  }
}