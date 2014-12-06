using System;

namespace JolTudomE_WP.Model {
  public class Statistic {

    public int CorrectAnswer { get;set; }
    public DateTime Generated { get; set; }
    public Nullable<decimal> Percent { get; set; }
    public Nullable<int> Questions { get; set; }
    public int TestID { get; set; }
    public Nullable<TimeSpan> TotalTime { get; set; }

    public string NumberOfQuestAnsw {
      get { return string.Format("{0}/{1}", CorrectAnswer, Questions); }
    }

    public string PercentString {
      get { return string.Format("{0:P}", Percent); }
    }

    public string GeneratedString {
      get { return Generated.ToString("g"); }
    }
  }

}

