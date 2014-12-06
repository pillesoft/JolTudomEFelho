using JolTudomE_WP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JolTudomE_WP.ViewModel {
  public class TestDetailViewModel : BaseNotifyable, IViewModel {

    private List<TestDetail> _TestDetList;

    public List<TestDetail> TestDetList {
      get { return _TestDetList; }
      set { SetProperty<List<TestDetail>>(ref _TestDetList, value); }
    }

    public TestDetailViewModel() {
      /*
       * design time data
      TestDetList = new List<TestDetail> {
        new TestDetail{QuestionText = "Ez a kerdes", ChekedAnswer = "Felhasznalo valasza", CorrectAnswer = "jo valasz"},
        new TestDetail{QuestionText = "Ez 2. a kerdes", ChekedAnswer = "Felhasznalo 2. valasza", CorrectAnswer = "jo 2. valasz"},
        new TestDetail{QuestionText = "Ez 3. a kerdes", ChekedAnswer = "3. valasz", CorrectAnswer = "3. valasz"},
      };
      */
    }

    public async void LoadData(object customdata) {
      try {
        var result = await DataSource.GetTestDetail((int)customdata);
        TestDetList = result;
      }
      catch { }
    }
  }
}
