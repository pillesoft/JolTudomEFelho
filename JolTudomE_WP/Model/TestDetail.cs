
namespace JolTudomE_WP.Model {
  public class TestDetail {
    public string QuestionText { get; set; }
    public string ChekedAnswer { get; set; }
    public string CorrectAnswer { get; set; }
    public bool IsGood { get { return ChekedAnswer == CorrectAnswer; } }
  }
}
