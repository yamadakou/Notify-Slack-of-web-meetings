using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace dcinc.api;

public class ExceptionLoggingMiddleware: IFunctionsWorkerMiddleware
{
	public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
	{
		throw new System.NotImplementedException();
	}
}