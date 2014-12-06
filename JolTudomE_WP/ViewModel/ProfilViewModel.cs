using JolTudomE_WP.Common;
using JolTudomE_WP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP.ViewModel {
  public class ProfilViewModel : BaseNotifyable {

    private UserDetail _UserProfil;
    public UserDetail UserProfil {
      get { return _UserProfil; }
      set {
        SetProperty<UserDetail>(ref _UserProfil, value);
        ShowProgressBar = false;
      }
    }

    private bool _ShowProgressBar;
    public bool ShowProgressBar {
      get { return _ShowProgressBar; }
      set { SetProperty<bool>(ref _ShowProgressBar, value); }
    }

    private RelayCommand _LoadData;
    public RelayCommand LoadData {
      get {
        return _LoadData
      ?? (_LoadData = new RelayCommand(
      async () => {
        ShowProgressBar = true;
        UserProfil = await DataSource.GetLoginDetail();
      },
      () => true));
      }
    }

    public ProfilViewModel() { }

  }
}
