namespace Incubation.AzConf.Filters;

public abstract class FunctionBase : IFunctionExceptionFilter, IFunctionInvocationFilter
{
    internal readonly ILogger<FunctionBase> _logger;

    // private readonly IHttpContextAccessor httpContextAccessor;

    public FunctionBase(ILogger<FunctionBase> log)
    {
        _logger = log;
        // IHttpContextAccessor httpContextAccessor
        // this.httpContextAccessor = httpContextAccessor;
    }

    public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
    {
        if (exceptionContext.Exception != null)
        {
            var ex = exceptionContext.Exception;
            WriteInnerException(ex);
            // var ex = exceptionContext.Exception;
            // httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // httpContextAccessor.HttpContext.Response.Headers.Append("key", "value");
            // await httpContextAccessor.HttpContext.Response.WriteAsync(ex.Message);
        }
        return Task.CompletedTask;
    }

    private void WriteInnerException(Exception ex)
    {
        while (ex != null)
        {
            _logger.LogError(ex.Message, ex);
            ex = ex.InnerException;
        }
    }

    public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}