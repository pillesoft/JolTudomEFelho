using System.Collections.Generic;

namespace JolTudomE_WP.Model {
  public class NewTest {
    public int TestID { get; set; }
    public int PersonID { get; set; }

    public List<NewTestQuestion> Questions { get; set; }

    public NewTest() {
      Questions = new List<NewTestQuestion>();
    }
  }

  public class NewTestQuestion {
    public int QuestionID { get; set; }
    public string QuestionText { get; set; }

    public List<NewTestQuestAnswer> Answers { get; set; }

    public NewTestQuestion() {
      Answers = new List<NewTestQuestAnswer>();
    }
  }

  public class NewTestQuestAnswer {
    public int AnswerID { get; set; }
    public string AnswerText { get; set; }
  }
}
