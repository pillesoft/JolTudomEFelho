using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JolTudomE_WP.Model {
  [DataContract]
  public class NewTestParam {
    [DataMember]
    public int NumberOfQuestions { get; set; }
    [DataMember]
    public List<int> TopicIDs { get; set; }
  }
}
