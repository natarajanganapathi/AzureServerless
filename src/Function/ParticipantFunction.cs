namespace Incubation.AzConf.Function;

public class ParticipantFunction : FunctionBase
{
    private readonly ParticipantRepository _repo;
    private readonly JsonSerializer _serializer;

    public ParticipantFunction(ILogger<ParticipantFunction> log, ParticipantRepository repo, JsonSerializer serializer) : base(log)
    {
        _repo = repo;
        _serializer = serializer;
    }

    [FunctionName(nameof(GetParticipants))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Participant" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Participent Id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Participant), Description = "The OK response")]
    public async Task<IActionResult> GetParticipants([HttpTrigger(AuthorizationLevel.Function, "get", Route = "participant-by-id")] HttpRequest req)
    {
        string id = req.Query["id"];
        var participant = await _repo.GetByIdAsync(new Guid(id));
        return new OkObjectResult(participant);
    }

    [FunctionName(nameof(CreateParticipants))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Participant" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("application/json", typeof(Participant))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Participant), Description = "The OK response")]
    public async Task<IActionResult> CreateParticipants([HttpTrigger(AuthorizationLevel.Function, "post", Route = "create-participant")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var participant = _serializer.Deserialize<Participant>(new JsonTextReader(new StringReader(requestBody)));
        participant.Attend = false;
        participant.EMail = participant?.EMail?.ToLower();
        await _repo.AddAsync(participant);
        return new OkObjectResult(participant);
    }

    [FunctionName(nameof(GetParticipantsByEmail))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Participant" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "email", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Participent email address")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Participant), Description = "The OK response")]
    public async Task<IActionResult> GetParticipantsByEmail([HttpTrigger(AuthorizationLevel.Function, "get", Route = "participant-by-email")] HttpRequest req)
    {
        string email = req.Query["email"];
        email = email?.ToLower();
        var participant = await _repo.GetByEmailAsync(email);
        if (participant == null)
        {
            // Call Microsoft API to identify the participant whether he registered or not.
            var res = await Validate.HasRegistered(email);
            participant = new Participant() { EMail = email, Enrolled = res, Attend = false };
        }
        return new OkObjectResult(participant);
    }


    [FunctionName(nameof(ExportParticipants))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Participant" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(void), Description = "The OK response")]
    public async Task ExportParticipants([HttpTrigger(AuthorizationLevel.Function, "get", Route = "export-participant")] HttpRequest req)
    {
        try
        {
            var pc = new PageContext(1);
            List<Participant> participants = (await _repo.GetAsync(pc: pc)).ToList();
            StringBuilder sb = new StringBuilder();
            while (participants.Count > 0)
            {
                foreach (var p in participants)
                {
                    sb.Append($"{p.Name},{p.Mobile},{p.EMail},{p.Role},{p.Enrolled},{p.Attend}{Environment.NewLine}");
                }
                participants = (await _repo.GetAsync(pc: pc.NextPage())).ToList();
            }
            File.WriteAllText("participants.csv", sb.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ExportParticipants");
        }
    }
}
