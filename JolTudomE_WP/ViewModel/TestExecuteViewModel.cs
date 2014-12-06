using JolTudomE_WP.Common;
using JolTudomE_WP.Model;
using System.Collections.Generic;

namespace JolTudomE_WP.ViewModel {
  public class TestExecuteViewModel : BaseNotifyable, IViewModel {

    private IEnumerator<NewTestQuestion> _TestQuestions;

    private NewTest _NewTest;
    public NewTest NewTest {
      get { return _NewTest; }
      set {
        SetProperty<NewTest>(ref _NewTest, value);
        _TestQuestions = _NewTest.Questions.GetEnumerator();
        
        _TestQuestions.MoveNext();
        ShowNewQuestion();
      }
    }

    private NewTestQuestion _CurrentQuestion;
    public NewTestQuestion CurrentQuestion {
      get { return _CurrentQuestion; }
      set {
        SetProperty<NewTestQuestion>(ref _CurrentQuestion, value);
      }
    }

    private string _TestQuestion;
    public string TestQuestion {
      get { return _TestQuestion; }
      set {
        SetProperty<string>(ref _TestQuestion, value);
      }
    }

    private string[] _Answers;
    public string[] Answers {
      get { return _Answers; }
      set { SetProperty<string[]>(ref _Answers, value); }
    }

    private int _CheckedAnswer;
    public int CheckedAnswer {
      get { return _CheckedAnswer; }
      set {
        SetProperty<int>(ref _CheckedAnswer, value);
        NextQuestionCommand.RaiseCanExecuteChanged();
        StopTestCommand.RaiseCanExecuteChanged();
      }
    }

    private RelayCommand _NextQuestionCommand;
    public RelayCommand NextQuestionCommand {
      get {
        return _NextQuestionCommand
      ?? (_NextQuestionCommand = new RelayCommand(
      async () => {
        await DataSource.AnswerTest(NewTest.TestID, CurrentQuestion.QuestionID, CurrentQuestion.Answers[CheckedAnswer - 1].AnswerID);
        _TestQuestions.MoveNext();
        ShowNewQuestion();
      },
      () => CanGoNext()));
      }
    }

    private RelayCommand _StopTestCommand;
    public RelayCommand StopTestCommand {
      get {
        return _StopTestCommand
      ?? (_StopTestCommand = new RelayCommand(
      async () => {
        int answerid = CurrentQuestion.Answers[CheckedAnswer - 1].AnswerID;
        await DataSource.CompleteTest(NewTest.TestID, CurrentQuestion.QuestionID, answerid);

        NavigationService.GoBack();
      },
      () => CheckedAnswer > 0));
      }
    }

    public TestExecuteViewModel() { }

    private bool CanGoNext() {
      return NewTest != null && 
        CheckedAnswer > 0 &&
        NewTest.Questions.IndexOf(_TestQuestions.Current) + 1 != NewTest.Questions.Count;
    }

    private void ShowNewQuestion() {
      CurrentQuestion = _TestQuestions.Current;
      TestQuestion = string.Format("{0}./{1}. {2}", NewTest.Questions.IndexOf(CurrentQuestion) + 1, NewTest.Questions.Count, CurrentQuestion.QuestionText);
      Answers = new string[] { 
        CurrentQuestion.Answers[0].AnswerText, 
        CurrentQuestion.Answers[1].AnswerText, 
        CurrentQuestion.Answers[2].AnswerText, 
        CurrentQuestion.Answers[3].AnswerText 
      };

      CheckedAnswer = 0;
      NextQuestionCommand.RaiseCanExecuteChanged();
    }

    public async void LoadData(object customdata) {

      if (SuspensionManager.SessionState.ContainsKey("CurrentTestID")) {
        DataSource.CurrentTest = int.Parse(SuspensionManager.SessionState["CurrentTestID"].ToString());
        DataSource.SelectedUserInfo = (LoginResponse)SuspensionManager.SessionState["SelectedUser"];
      }

      try {
        NewTest result;
        if (DataSource.HasCurrentTest) {
          ((App)App.Current).ShowDialog("Teszt Folytatása", "Az alkalmazást megszakították egy teszt végrehajtása közben.\nA hátramaradt kérdéseket most megválaszolhatja, vagy szakítsa meg a tesztet!");

          result = await DataSource.ContinueTest();
          SuspensionManager.SessionState.Remove("CurrentTestID");
        }
        else {
          result = await DataSource.GenerateTest();
        }
        NewTest = result;
      }
      catch { }

    }
  }
}
