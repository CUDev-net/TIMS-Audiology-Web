using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TIMS_X.Server;

public class Program
{
	public static IHostBuilder CreateHostBuilder(string[] args)
	{
		return Host.CreateDefaultBuilder(args)
			.UseDefaultServiceProvider(options =>
				options.ValidateScopes = false)
			.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
	}

	public static void Main(string[] args)
	{
		CreateHostBuilder(args).Build().Run();
	}
}