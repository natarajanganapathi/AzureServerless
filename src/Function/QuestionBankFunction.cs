namespace Incubation.AzConf.Function;

public class QuestionBankFunction : FunctionBase
{
    private readonly QuestionBankRepository _repo;
    private readonly CompositeService _cs;

    public QuestionBankFunction(ILogger<QuestionBankFunction> log, CompositeService cs, QuestionBankRepository repo) : base(log)
    {
        _cs = cs;
        _repo = repo;
    }

    [FunctionName(nameof(GetQuestionsByCategory))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "QuestionBank" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "category", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Question's category name")]
    [OpenApiParameter(name: "participantId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Participant Id")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> GetQuestionsByCategory([HttpTrigger(AuthorizationLevel.Function, "get", Route = "questions")] HttpRequest req)
    {
        string category = req.Query["category"];
        string participantId = req.Query["participantId"];
        await _cs.ExamAttendByAsync(participantId);
        var filter = _repo.GetFilterDef().Eq("Category", category);
        var questions = (await _repo.GetAsync(filter: filter)).ToList();
        if (questions.Count < 5)
        {
            throw new Exception($"There is no enough questions available in the catgory {category}");
        }
        var uniqueRandom = UniqueRandom.Get(5, questions.Count);
        var response = uniqueRandom.Select(x => new QuestionBank()
        {
            QuestionType = questions[x].QuestionType,
            Question = questions[x].Question,
            Category = questions[x].Category,
            Options = questions[x].Options,
            Id = questions[x].Id

        });
        return new OkObjectResult(response);
    }

    [FunctionName(nameof(GetCategories))]
    [OpenApiOperation(operationId: "Run", tags: new[] { "QuestionBank" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public IActionResult GetCategories([HttpTrigger(AuthorizationLevel.Function, "get", Route = "category")] HttpRequest req)
    {
        var response = _repo.GetCategory();
        return new OkObjectResult(response);
    }

    [FunctionName(nameof(UploadQuestions))]
    [OpenApiOperation(operationId: "run", tags: new[] { "QuestionBank" }, Summary = "Transfer Excel file multipart/formdata", Description = "This transfers an image through multipart/formdata.", Visibility = OpenApiVisibilityType.Advanced)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(MultiPartFormDataModel), Required = true, Description = "Excel File data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "image/png", bodyType: typeof(byte[]), Summary = "Excel file data", Description = "This returns the image", Deprecated = false)]
    public async Task<IActionResult> UploadQuestions(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload-questions")] HttpRequest req, ILogger log)
    {
        var file = req.Form.Files[0];
        var data = new List<Dictionary<string, Object>>();
        if (file != null)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms).ConfigureAwait(false);
                data = ExcelFile.Read(ms, "Questions");
            }
        }
        var input = data.Select(x =>
        {
            QuestionType qt;
            if(!Enum.TryParse(x[nameof(QuestionBank.QuestionType)].ToString(), out qt))
            {
                qt = QuestionType.Multi;
            }
            var optionKeys = x.Keys.Where(x => x.StartsWith("Options", true, System.Globalization.CultureInfo.CurrentCulture));
            var options = optionKeys.Select(options =>
            {
                var value = options.Split(".");
                return new Answer() { Id = Int32.Parse(value[1]), Value = x[options].ToString() };
            }).ToList();

            var answerKeys = x.Keys.Where(ans => ans.StartsWith("Answer", true, System.Globalization.CultureInfo.CurrentCulture));
            var answers = answerKeys.Select(ansKey =>
            {
                var value = x[ansKey]?.ToString()?.Split(".");
                return Int32.Parse(value[1]);
            }).ToList();
            if (qt == QuestionType.Single && answers.Count > 1)
            {
                throw new Exception($"Question Type is single. But Multiple answers are selected");
            }
            return new QuestionBank()
            {
                QuestionType = qt,
                Category = x[nameof(QuestionBank.Category)].ToString(),
                Question = x[nameof(QuestionBank.Question)].ToString(),
                Options = options,
                Answers = answers,
            };
        }).ToList();
        await _repo.AddManyAsync(input);
        return new OkObjectResult("Ok");
    }
}
