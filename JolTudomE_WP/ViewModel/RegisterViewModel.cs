using JolTudomE_WP.Common;
using JolTudomE_WP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP.ViewModel {
  class RegisterViewModel : BaseNotifyable, IViewModel {

    private UserDetail _NewUser;
    public UserDetail NewUser {
      get { return _NewUser; }
      set {
        SetProperty<UserDetail>(ref _NewUser, value);
        RegisterCommand.RaiseCanExecuteChanged();
      }
    }

    private RelayCommand _RegisterCommand;
    public RelayCommand RegisterCommand {
      get {
        return _RegisterCommand
      ?? (_RegisterCommand = new RelayCommand(
      async () => {
        if (!NewUser.IsValid) {
          ((App)App.Current).ShowDialog("Regisztrálási Hiba", "A beviteli mezők adatai hibásak!");
        }
        else {
          IsProgressBarVisible = true;
          try {
            await DataSource.MakeRegister(NewUser);
            ((App)App.Current).ShowDialog("Regisztráció", "A regisztráció sikerült!");
            NavigationService.NavigateTo(PageEnum.Login);
          }
          catch (WebApiException mexc) {
            IsProgressBarVisible = false;
            ((App)App.Current).ShowDialog("Regisztrálási Hiba", mexc.Message);
          }
          catch (Exception exc) {
            IsProgressBarVisible = false;
            ((App)App.Current).ShowDialog("Regisztrálási Hiba", exc.Message);
          }
        }
      },
      () => true));
      }
    }

    private RelayCommand _CancelCommand;
    public RelayCommand CancelCommand {
      get {
        return _CancelCommand
      ?? (_CancelCommand = new RelayCommand(
      () => {
        NavigationService.NavigateTo(PageEnum.Login);
      },
      () => true));
      }
    }

    private bool _IsProgressBarVisible;
    public bool IsProgressBarVisible {
      get { return _IsProgressBarVisible; }
      set { SetProperty<bool>(ref _IsProgressBarVisible, value); }
    }


    public void LoadData(object customdata) {
      NewUser = (UserDetail)customdata;
    }
  }
}
