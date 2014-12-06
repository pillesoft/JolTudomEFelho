using JolTudomE_WP.Common;
using JolTudomE_WP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace JolTudomE_WP.ViewModel {
  public class LoggedInUserViewModel : BaseNotifyable, IViewModel {

    private int _SelectedPivotIndex;
    public int SelectedPivotIndex {
      get { return _SelectedPivotIndex; }
      set {
        SetProperty<int>(ref _SelectedPivotIndex, value);
        IsCommandBarVisible = _SelectedPivotIndex == 1;
      }
    }

    private bool _IsCommandBarVisible;
    public bool IsCommandBarVisible {
      get { return _IsCommandBarVisible; }
      set { SetProperty<bool>(ref _IsCommandBarVisible, value); }
    }

    private RelayCommand _CredentialClearCommand;
    public RelayCommand CredentialClearCommand {
      get {
        return _CredentialClearCommand
      ?? (_CredentialClearCommand = new RelayCommand(
      () => {
        ((App)App.Current).ClearCredential();
      },
      () => true));
      }
    }

    private bool _ShowProgressBar;
    public bool ShowProgressBar {
      get { return _ShowProgressBar; }
      set { SetProperty<bool>(ref _ShowProgressBar, value); }
    }
    
    private List<GroupedUser> _UserList;
    public List<GroupedUser> UserList {
      get { return _UserList; }
      set {
        SetProperty<List<GroupedUser>>(ref _UserList, value);
        if (_UserList != null) {
          var result = from user in _UserList group user by user.Role into grp orderby grp.Key.GroupingOrder select grp;
          UserListGrouped.Source = result;
        }
      }
    }

    private RelayCommand<GroupedUser> _ItemClickedCommand;
    public RelayCommand<GroupedUser> ItemClickedCommand {
      get {
        return _ItemClickedCommand
      ?? (_ItemClickedCommand = new RelayCommand<GroupedUser>(
      (gu) => {
        DataSource.SelectedUserInfo = new Model.LoginResponse {
          DisplayName = gu.DisplayName,
          PersonID = gu.PersonID,
          RoleID = gu.Role.RoleID
        };

        NavigationService.NavigateTo(PageEnum.SelectedUser);
      },
      (gu) => true));
      }
    }

    private CollectionViewSource _UserListGrouped;
    public CollectionViewSource UserListGrouped {
      get { return _UserListGrouped; }
      set { SetProperty<CollectionViewSource>(ref _UserListGrouped, value); }
    }
    
    public LoggedInUserViewModel() {

      UserListGrouped = new CollectionViewSource();
      UserListGrouped.IsSourceGrouped = true;

    }

    public async void LoadData(object customdata) {
      ShowProgressBar = true;

      UserList = await DataSource.GetUserList();

      ShowProgressBar = false;
    }
  }
}
