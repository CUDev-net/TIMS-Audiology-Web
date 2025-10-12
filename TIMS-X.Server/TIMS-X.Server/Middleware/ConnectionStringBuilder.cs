using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Models;

namespace TIMS_X.Server.Middleware;

public static class ConnectionStringBuilder
{
	public static string GetConnectionString(string server, string database, string username, string password,
		bool encrypt = false)
	{
		// Create the connection string
		var connectionString = $"server={server};" +
		                       "packet size=4096;" +
		                       $"user id={username};" +
		                       $"data source={server};" +
		                       "persist security info=True;" +
		                       "connect timeout=15;" +
		                       "MultipleActiveResultSets=True;" +
		                       $"initial catalog={database};" +
		                       $"password={password};" +
		                       $"Encrypt={encrypt};";
#if DEBUG
		if (encrypt) connectionString = connectionString + "TrustServerCertificate=True;";
#endif
		return connectionString;
	}

	public static void SetConnectionString(DbContextOptionsBuilder options, ConnectionInfo connInfo)
	{
		SetConnectionString(options, connInfo.Server, connInfo.Database, connInfo.User, connInfo.Password);
	}

	public static void SetConnectionString(DbContextOptionsBuilder options, string server, string database,
		string username, string password, bool encrypt = false)
	{
		var connectionString = GetConnectionString(server, database, username, password, encrypt);
		// Pass it to the db context
		options.UseSqlServer(connectionString /*, builder => builder.EnableRetryOnFailure()*/);
	}
}