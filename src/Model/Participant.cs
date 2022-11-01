namespace Incubation.AzConf.Model;

[Collection("Participant")]
public class Participant : BaseModel
{
    public string Name { get; set; }
    public string EMail { get; set; }
    public string Mobile { get; set; }
    public string Role { get; set; }
    public Boolean Enrolled { get; set; }
    public Boolean Attend { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
