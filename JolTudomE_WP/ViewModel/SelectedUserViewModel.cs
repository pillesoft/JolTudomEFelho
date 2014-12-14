using JolTudomE_WP.Common;
using JolTudomE_WP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage.Pickers;

namespace JolTudomE_WP.ViewModel {
  public class SelectedUserViewModel : BaseNotifyable, IViewModel {

    private int _SelectedPivotIndex;
    public int SelectedPivotIndex {
      get { return _SelectedPivotIndex; }
      set {
        SetProperty<int>(ref _SelectedPivotIndex, value);
        IsCommandBarVisible = _SelectedPivotIndex == 2;
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

    private RelayCommand _PickupImageCommand;
    public RelayCommand PickupImageCommand {
      get {
        return _PickupImageCommand
      ?? (_PickupImageCommand = new RelayCommand(
      () => {
        FileOpenPicker openPicker = new FileOpenPicker();
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".jpg");
        openPicker.FileTypeFilter.Add(".jpeg");
        openPicker.FileTypeFilter.Add(".png");

        // Launch file open picker and caller app is suspended and may be terminated if required
        openPicker.PickSingleFileAndContinue();
      },
      () => true));
      }
    }

    private bool _ShowProgressBar;
    public bool ShowProgressBar {
      get { return _ShowProgressBar; }
      set { SetProperty<bool>(ref _ShowProgressBar, value); }
    }

    private List<Statistic> _StatisticList;
    public List<Statistic> StatisticList {
      get { return _StatisticList; }
      set {
        SetProperty<List<Statistic>>(ref _StatisticList, value);
        IsStatisticEmpty = _StatisticList != null && _StatisticList.Count == 0;
      }
    }

    private bool _IsStatisticEmpty;
    public bool IsStatisticEmpty {
      get { return _IsStatisticEmpty; }
      set { SetProperty<bool>(ref _IsStatisticEmpty, value); }
    }

    private ObservableCollection<Course> _CourseList;
    public ObservableCollection<Course> CourseList {
      get { return _CourseList; }
      set { 
        SetProperty<ObservableCollection<Course>>(ref _CourseList, value);
        if (_CourseList != null) SelectedCourse = _CourseList.First();
      }
    }

    private ObservableCollection<Topic> _TopicList;
    public ObservableCollection<Topic> TopicList {
      get { return _TopicList; }
      set { SetProperty<ObservableCollection<Topic>>(ref _TopicList, value); }
    }

    private Course _SelectedCourse;
    public Course SelectedCourse {
      get { return _SelectedCourse; }
      set {
        SetProperty<Course>(ref _SelectedCourse, value);
        GetTopicList();
      }
    }

    private int _NumberQuestion;
    public int NumberQuestion {
      get { return _NumberQuestion; }
      set { SetProperty<int>(ref _NumberQuestion, value); }
    }

    private List<int> _SelectedTopics;
    public List<int> SelectedTopics {
      get { return _SelectedTopics; }
      set { _SelectedTopics = value; }
    }

    private bool _IsTopicErrorShown;
    public bool IsTopicErrorShown {
      get { return _IsTopicErrorShown; }
      set { SetProperty<bool>(ref _IsTopicErrorShown, value); }
    }

    private string _SelectedUser;
    public string SelectedUser {
      get { return _SelectedUser; }
      set { SetProperty<string>(ref _SelectedUser, value); }
    }

    private RelayCommand<Statistic> _ItemClickedCommand;
    public RelayCommand<Statistic> ItemClickedCommand {
      get {
        return _ItemClickedCommand
      ?? (_ItemClickedCommand = new RelayCommand<Statistic>(
      (st) => {
        int testid = st.TestID;
        NavigationService.NavigateTo(PageEnum.TestDetail, testid);
      },
      (st) => true));
      }
    }

    private RelayCommand _StartTestCommand;
    public RelayCommand StartTestCommand {
      get {
        return _StartTestCommand
      ?? (_StartTestCommand = new RelayCommand(
      () => {
        DataSource.AddNewTestParam(new NewTestParam { NumberOfQuestions = NumberQuestion, TopicIDs = SelectedTopics });
        NavigationService.NavigateTo(PageEnum.TestExecute);
      },
      () => SelectedTopics.Count > 0));
      }
    }

    private RelayCommand<List<int>> _SelectionChangedCommand;
    public RelayCommand<List<int>> SelectionChangedCommand {
      get {
        return _SelectionChangedCommand
      ?? (_SelectionChangedCommand = new RelayCommand<List<int>>(
      (tidlist) => {
        SelectedTopics = new List<int>();
        foreach (int id in tidlist) {
          SelectedTopics.Add(id);
        }
        IsTopicErrorShown = SelectedTopics.Count == 0;
        StartTestCommand.RaiseCanExecuteChanged();
      },
      (tidlist) => true));
      }
    }

    public SelectedUserViewModel() {

      NumberQuestion = 15;
      IsTopicErrorShown = false;
      SelectedUser = "Belépett Felhasználó";

    }

    private async void GetTopicList() {
      TopicList = await DataSource.GetTopics(_SelectedCourse.CourseID);
      SelectedTopics = new List<int>();
      IsTopicErrorShown = false;
    }


    public async void LoadData(object customdata) {
      ShowProgressBar = true;
      
      SelectedUser = DataSource.SelectedUserInfo.DisplayName;

      StatisticList = await DataSource.GetStatistic();
      CourseList = await DataSource.GetCourses();

      ShowProgressBar = false;
    }
  }
}
