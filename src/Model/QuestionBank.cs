namespace Incubation.AzConf.Model;
[Collection("QuestionBank")]
public class QuestionBank : BaseModel
{
    public QuestionType QuestionType { get; set; }
    public String Category { get; set; }
    public String Question { get; set; }
    public List<Answer> Options { get; set; }
    public List<Int32> Answers { get; set; }
}

public enum QuestionType
{
    Single,
    Multi
}
public class Answer
{
    public Int32 Id { get; set; }
    public String Value { get; set; }
}
