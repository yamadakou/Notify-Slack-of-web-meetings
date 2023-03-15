using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace dcinc.api
{
	public class Program
	{
		static Task Main()
		{
			var host = new HostBuilder()
				.ConfigureFunctionsWorkerDefaults(
					builder =>
					{
						builder.UseMiddleware<ExceptionLoggingMiddleware>();
					}
				)
				.Build();

			return host.RunAsync();
		}
	}
}
