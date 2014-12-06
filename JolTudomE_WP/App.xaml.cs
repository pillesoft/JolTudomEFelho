using JolTudomE_WP.Common;
using JolTudomE_WP.View;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.Storage;
using Windows.Security.Credentials;
using System.Threading.Tasks;
using JolTudomE_WP.Model;
using LoggerWP;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace JolTudomE_WP {

  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public sealed partial class App : Application {
    private TransitionCollection transitions;
    private bool IsDialogOpen;

    private const string _CredLockerResource = "JolTudomE";

    public static Logger LogIt;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() {
      this.InitializeComponent();
      this.Suspending += this.OnSuspending;
      this.Resuming += this.OnResuming;
      this.UnhandledException += App_UnhandledException;

    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used when the application is launched to open a specific file, to display
    /// search results, and so forth.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs e) {
#if DEBUG
      if (System.Diagnostics.Debugger.IsAttached) {
        this.DebugSettings.EnableFrameRateCounter = true;
      }
#endif

      LogIt = new Logger();
      try {
        await LogIt.Init("JolTudomE");
      }
      catch { }
      if (!LogIt.IsReady) {
        ShowDialog("Jól Tudom E?", "Az alkalmazás teljes értékű futtatásához kell SD kártya!");
      } 

      Frame rootFrame = Window.Current.Content as Frame;

      // Do not repeat app initialization when the Window already has content,
      // just ensure that the window is active.
      if (rootFrame == null) {
        // Create a Frame to act as the navigation context and navigate to the first page.
        rootFrame = new Frame();

        // Associate the frame with a SuspensionManager key.
        SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

        SuspensionManager.KnownTypes.Add(typeof(LoginResponse));

        // TODO: Change this value to a cache size that is appropriate for your application.
        rootFrame.CacheSize = 1;

        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
          LogIt.LogInfo("App-RestoreFromTerminate");

          // try to authenticate the user, who was in when the termination is occured
          PasswordCredential cred = GetCredential();
          if (cred != null) {
            cred.RetrievePassword();
            try {
              await DataSource.MakeLogin(cred.UserName, cred.Password, () => { });
            }
            catch { }
          }

          if (DataSource.IsAuthenticated) {
            // Restore the saved session state only when appropriate.
            // and if the previous login attempt succeeded
            try {
              await SuspensionManager.RestoreAsync();
            }
            catch (SuspensionManagerException) {
              // Something went wrong restoring state.
              // Assume there is no state and continue.
            }
          }
        }

        // Place the frame in the current Window.
        Window.Current.Content = rootFrame;
      }

      if (rootFrame.Content == null) {
        // Removes the turnstile navigation for startup.
        if (rootFrame.ContentTransitions != null) {
          this.transitions = new TransitionCollection();
          foreach (var c in rootFrame.ContentTransitions) {
            this.transitions.Add(c);
          }
        }

        rootFrame.ContentTransitions = null;
        rootFrame.Navigated += this.RootFrame_FirstNavigated;

        // When the navigation stack isn't restored navigate to the first page,
        // configuring the new page by passing required information as a navigation
        // parameter.

        Authenticate();

      }

      // Ensure the current window is active.
      Window.Current.Activate();
    }

    private async void Authenticate() {
      PasswordCredential cred = GetCredential();
      if (cred == null) {
        NavigationService.NavigateTo(PageEnum.Login);
      }
      else {
        cred.RetrievePassword();
        try {
          await DataSource.MakeLogin(cred.UserName, cred.Password,
            () => {
              if (DataSource.SelectedUserInfo != null 
                && DataSource.LoggedInInfo.PersonID == DataSource.SelectedUserInfo.PersonID) {
                NavigationService.NavigateTo(PageEnum.SelectedUser);
              }
              else {
                NavigationService.NavigateTo(PageEnum.LoggedInUser);
              }
            });

        }
        catch (UnauthorizedException) {
          ShowDialog("Bejelentkezési Hiba", "Az eltárolt Felhasználó név/Jelszó már nem megfelelő!");
          NavigationService.NavigateTo(PageEnum.Login);
        }
        catch (Exception exc) {
          ShowDialog("Bejelentkezési Hiba", exc.Message);
          NavigationService.NavigateTo(PageEnum.Login);
        }

      }

    }

    /// <summary>
    /// Restores the content transitions after the app has launched.
    /// </summary>
    private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e) {
      var rootFrame = sender as Frame;
      rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
      rootFrame.Navigated -= this.RootFrame_FirstNavigated;
    }

    /// <summary>
    /// Invoked when application execution is being suspended.  Application state is saved
    /// without knowing whether the application will be terminated or resumed with the contents
    /// of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private async void OnSuspending(object sender, SuspendingEventArgs e) {
      var deferral = e.SuspendingOperation.GetDeferral();
      LogIt.LogInfo("App-Suspending");

      if (DataSource.HasCurrentTest) {
        SuspensionManager.SessionState["CurrentTestID"] = (int)DataSource.CurrentTest;
        SuspensionManager.SessionState["SelectedUser"] = DataSource.SelectedUserInfo;
        await DataSource.SuspendTest();
      }

      await SuspensionManager.SaveAsync();
      deferral.Complete();
    }

    async void OnResuming(object sender, object e) {
      LogIt.LogInfo("App-Resuming");

      if (DataSource.HasCurrentTest) {
        SuspensionManager.SessionState.Remove("CurrentTestID");

        await DataSource.ResumeTest();
      }
    }

    void App_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      LogIt.LogError(string.Format("Unhandled Exception occured: {0}", e.Exception.ToString()));
      e.Handled = true;
      ShowDialog("Unhandled Exception", e.Message);
    }

    public void SessionExpired() {
      if (!IsDialogOpen) {

        ShowDialog("Lejárt a Session!", "Az éppen aktuális munkamenet lejárt! Újra be kell jelentkezni ...");

        Frame rootFrame = Window.Current.Content as Frame;
        rootFrame.BackStack.Clear();

        Authenticate();
      }
    }

    public async void ShowDialog(string title, string msg) {
      if (!IsDialogOpen) {
        IsDialogOpen = true;

        WPDialog dialog = new WPDialog(msg) {
          Title = title
        };
        await dialog.ShowAsync();

        IsDialogOpen = false;
      }
    }

    #region Credential

    public void SaveCredential(string username, string password) {
      PasswordVault vault = new PasswordVault();
      
      ClearCredential();

      // save the new credential
      PasswordCredential cred = new PasswordCredential(_CredLockerResource, username, password);
      vault.Add(cred);
    }

    public void ClearCredential() {
      PasswordVault vault = new PasswordVault();
      // remove the already saved credentials
      try {
        var credcoll = vault.FindAllByResource(_CredLockerResource);
        foreach (var item in credcoll) {
          vault.Remove(item);
        }
      }
      catch { /* that means there is no saved credential */}
    }

    private PasswordCredential GetCredential() {
      PasswordVault vault = new PasswordVault();
      PasswordCredential cred = null;
      try {
        var credcoll = vault.FindAllByResource(_CredLockerResource);
        cred = credcoll[0];
      }
      catch (Exception) { }
      return cred;
    }
    #endregion

  }
}
