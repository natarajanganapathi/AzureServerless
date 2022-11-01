namespace Incubation.AzConf.Repository;

public class CompositeService
{
    private readonly ILogger<LeaderBoardFunction> _logger;
    private readonly LeaderBoardRepository _lbRepo;
    private readonly QuestionBankRepository _qbRepo;
    private readonly ParticipantRepository _pRepo;
    private readonly JsonSerializer _serializer;
    public CompositeService(ILogger<LeaderBoardFunction> log, LeaderBoardRepository lbRepo,
    ParticipantRepository pRepo, QuestionBankRepository qbRepo, JsonSerializer serializer)
    {
        _logger = log;
        _lbRepo = lbRepo;
        _qbRepo = qbRepo;
        _pRepo = pRepo;
        _serializer = serializer;
    }
    public async Task ExamAttendByAsync(string participantId)
    {
        var filter = Builders<Participant>
            .Filter
            .Eq("_id", ObjectId.Parse(participantId));
        var update = Builders<Participant>.Update
            .Set(nameof(Participant.StartTime), DateTime.Now)
            .Set(nameof(Participant.Attend), true);
        await _pRepo.UpdateAsync(filter, update);
    }

    public async Task<Participant> ExamCompletedByAsync(string participantId)
    {
        var filter = Builders<Participant>
            .Filter
            .Eq("_id", ObjectId.Parse(participantId));
        var update = Builders<Participant>.Update
            .Set(nameof(Participant.EndTime), DateTime.Now);
        var result = await _pRepo.UpdateAsync(filter, update);
        return await _pRepo.GetByIdAsync(new Guid(participantId));
    }
    public async Task<Int32> CalculateScore(LeaderBoard participant)
    {
        var ids = participant.QuestionDetails.Select(x => x.QuestionId).ToArray();
        var qb = await _qbRepo.GetByIdsAsync(ids);
        var source = qb.ToDictionary(x => x.Id, x => x);
        var score = 0;
        foreach (var question in participant.QuestionDetails)
        {
            // Todo: Performance optimization required
            var originalAns = source[question.QuestionId].Answers.OrderBy(x => x);
            var ans = question.Answers.OrderBy(x => x);
            if (originalAns.SequenceEqual(ans))
            {
                score++;
            }
        }
        return score;
    }

    public async Task ExportAllDataAsJson()
    {
        _logger.LogInformation("Exporting all data as json");
        await ExportAllParticipantDataAsJson();
        await ExportAllQuestionBankDataAsJson();
        await ExportAllLeaderBoardDataAsJson();
        _logger.LogInformation("Exporting all data as json completed");
    }

    private async Task ExportAllLeaderBoardDataAsJson()
    {
        var leaderBoardList = new List<LeaderBoard>();
        var pc = new PageContext(1);
        List<LeaderBoard> leaderboards = (await _lbRepo.GetAsync(pc: pc)).ToList();
        while (leaderboards.Count > 0)
        {
            leaderBoardList.AddRange(leaderboards);
            leaderboards = (await _lbRepo.GetAsync(pc: pc.NextPage())).ToList();
        }
        using (StreamWriter sw = new StreamWriter(@"leaderboards.json"))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            _serializer.Serialize(writer, leaderBoardList);
        }
    }
    private async Task ExportAllQuestionBankDataAsJson()
    {
        var questionBankList = new List<QuestionBank>();
        var pc = new PageContext(1);
        List<QuestionBank> questionBanks = (await _qbRepo.GetAsync(pc: pc)).ToList();
        while (questionBanks.Count > 0)
        {
            questionBankList.AddRange(questionBanks);
            questionBanks = (await _qbRepo.GetAsync(pc: pc.NextPage())).ToList();
        }
        using (StreamWriter sw = new StreamWriter(@"questions.json"))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            _serializer.Serialize(writer, questionBankList);
        }
    }
    private async Task ExportAllParticipantDataAsJson()
    {
        var participantsList = new List<Participant>();
        var pc = new PageContext(1);
        List<Participant> participants = (await _pRepo.GetAsync(pc: pc)).ToList();
        while (participants.Count > 0)
        {
            participantsList.AddRange(participants);
            participants = (await _pRepo.GetAsync(pc: pc.NextPage())).ToList();
        }
        using (StreamWriter sw = new StreamWriter(@"participants.json"))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            _serializer.Serialize(writer, participantsList);
        }
    }
}
