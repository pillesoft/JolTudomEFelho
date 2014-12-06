using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LoggerWP {

  public enum LogLevel {
    Info,
    Debug,
    Error
  }

  public sealed class Logger : IDisposable {
    private StorageFolder _SDCard;
    private StorageFolder _LogFolder;
    private string _FileName;
    private Stream _Stream;

    public Logger() {
    }

    public bool IsReady {
      get { return _SDCard != null; }
    }

    public async Task Init(string appname) {
      var devices = Windows.Storage.KnownFolders.RemovableDevices;
      var sdCards = await devices.GetFoldersAsync();
      if (sdCards.Count == 0) return;
      _SDCard = sdCards[0];
      string foldername = string.Format("{0}_Log", appname);
      _LogFolder = await _SDCard.CreateFolderAsync(foldername, CreationCollisionOption.OpenIfExists);

      _FileName = string.Format("{0:yyyy-MM-dd}.txt", DateTime.Now);
      _Stream = await _LogFolder.OpenStreamForWriteAsync(_FileName, CreationCollisionOption.OpenIfExists);

    }

    private async Task Write(string msg) {
      try {
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(msg));
        byte[] msgbyte = ms.ToArray();

        await _Stream.WriteAsync(msgbyte, 0, msgbyte.Length);
        await _Stream.FlushAsync();

      }
      catch (NullReferenceException) {}
    }

    private async void Logit(string msg, LogLevel level) {
      if (!IsReady) return;
      string formattedmsg = string.Format("{0:yyyy-MM-dd HH:mm:ss} {1} {2}\n", DateTime.Now, level, msg);
      await Write(formattedmsg);
    }

    public void LogInfo(string msg) {
      Logit(msg, LogLevel.Info);
    }

    public void LogError(string msg) {
      Logit(msg, LogLevel.Error);
    }

    public void LogDebug(string msg) {
      Logit(msg, LogLevel.Debug);
    }

    public void Dispose() {
      _Stream.Dispose();
    }
  }
}
