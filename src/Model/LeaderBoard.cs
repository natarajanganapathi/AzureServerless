namespace Incubation.AzConf.Model;

[Collection("LeaderBoard")]
public class LeaderBoard : BaseModel
{
    public string ParticipantId { get; set; }
    public string Name { get; set; }
    public string EMail { get; set; }
    public Int32 Score { get; set; }
    public List<QuestionDetail> QuestionDetails { get; set; }
    public Double TimeTaken { get; set; }
    public Int32 Rank { get; set; }
    public Boolean ReceivedGoodies { get; set; }
}

public class QuestionDetail
{
    public string QuestionId { get; set; }
    public List<Int32> Answers { get; set; }
}
