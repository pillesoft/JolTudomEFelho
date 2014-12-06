using JolTudomE_WP.View;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JolTudomE_WP {

  public enum PageEnum {
    Login,
    Register,
    SelectedUser,
    LoggedInUser,
    TestDetail,
    TestExecute
  }

  public static class NavigationService {
    private static Frame _RootFrame = Window.Current.Content as Frame;
    private static Dictionary<PageEnum, Type> _PageMapping = new Dictionary<PageEnum, Type>() {
      {PageEnum.Login, typeof(LoginPage)},
      {PageEnum.Register, typeof(RegisterPage)},
      {PageEnum.SelectedUser, typeof(SelectedUserPage)},
      {PageEnum.LoggedInUser, typeof(LoggedInUserPage)},
      {PageEnum.TestDetail, typeof(TestDetailPage)},
      {PageEnum.TestExecute, typeof(TestExecutePage)}
    };

    public static void NavigateTo(PageEnum page, object param) {
      _RootFrame.Navigate(_PageMapping[page], param);
    }
    public static void NavigateTo(PageEnum page) {
      _RootFrame.Navigate(_PageMapping[page]);
    }

    public static void GoBack() {
      _RootFrame.GoBack();
    }
  }
}
