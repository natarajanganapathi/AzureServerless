namespace Incubation.AzConf.Function;

public class LeaderBoardFunction : FunctionBase
{
    private readonly LeaderBoardRepository _repo;
    private readonly JsonSerializer _serializer;
    private readonly CompositeService _compositeService;
    public LeaderBoardFunction(ILogger<LeaderBoardFunction> log, LeaderBoardRepository repo,
                JsonSerializer serializer, CompositeService compositeService) : base(log)
    {
        _repo = repo;
        _serializer = serializer;
        _compositeService = compositeService;
    }

    [FunctionName(nameof(GetLeaderBoardDetailsByParticipantId))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "LeaderBoard" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "participantId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Participant ID")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LeaderBoard), Description = "The OK response")]
    public async Task<IActionResult> GetLeaderBoardDetailsByParticipantId([HttpTrigger(AuthorizationLevel.Function, "get", Route = "get-leader-board-by-participant-id")] HttpRequest req)
    {
        string id = req.Query["participantId"];
        var participant = await _repo.GetLeaderBoardDetailsByParticipantId(id);
        return new OkObjectResult(participant);
    }

    [FunctionName(nameof(GetLeaderBoard))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "LeaderBoard" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("applicatoin/json", typeof(PageContext))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LeaderBoard>), Description = "The OK response")]
    public async Task<IActionResult> GetLeaderBoard([HttpTrigger(AuthorizationLevel.Function, "post", Route = "leader-board")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var pc = _serializer.Deserialize<PageContext>(new JsonTextReader(new StringReader(requestBody)));
        var response = await _repo.GetLeaderBoard(pc);
        return new OkObjectResult(response);
    }

    [FunctionName(nameof(ParticipantAnswers))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "LeaderBoard" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("applicatoin/json", typeof(LeaderBoard))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LeaderBoard), Description = "The OK response")]
    public async Task<IActionResult> ParticipantAnswers([HttpTrigger(AuthorizationLevel.Function, "post", Route = "participant-answers")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var answer = _serializer.Deserialize<LeaderBoard>(new JsonTextReader(new StringReader(requestBody)));
        var participant = await _compositeService.ExamCompletedByAsync(answer.ParticipantId);
        answer.Name = participant.Name;
        answer.TimeTaken = participant.EndTime.Subtract(participant.StartTime).TotalMilliseconds;
        answer.Score = await _compositeService.CalculateScore(answer);
        answer.ReceivedGoodies = false;
        await _repo.AddAsync(answer);
        return new OkObjectResult(answer);
    }

    [FunctionName(nameof(ReceivedGoddies))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "LeaderBoard" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "participantId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Participent Id")]
    [OpenApiParameter(name: "received", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "Recived Goodies?")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(void), Description = "The OK response")]
    public async Task ReceivedGoddies([HttpTrigger(AuthorizationLevel.Function, "patch", Route = "received-goodies")] HttpRequest req)
    {
        string id = req.Query["participantId"];
        bool received = Convert.ToBoolean(req.Query["received"]);
        await _repo.ReceivedGoodies(id, received);
    }

    [FunctionName(nameof(ParticipantCount))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "LeaderBoard" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    // [OpenApiParameter(name: "received", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Description = "Count participant")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(long), Description = "The OK response")]
    public async Task<IActionResult> ParticipantCount([HttpTrigger(AuthorizationLevel.Function, "get", Route = "participant-count")] HttpRequest req)
    {
        var count = await _repo.GetCountAsync();
        return new OkObjectResult(count);
    }

    [FunctionName(nameof(ExportAllDataAsJson))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "LeaderBoard" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(void), Description = "The OK response")]
    public async Task ExportAllDataAsJson([HttpTrigger(AuthorizationLevel.Function, "get", Route = "export-all-data-as-json")] HttpRequest req)
    {
        try
        {
           await _compositeService.ExportAllDataAsJson();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ExportAllDataAsJson");
        }
    }
}
