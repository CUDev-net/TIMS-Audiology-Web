using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace TIMS_X.Server.Extensions;

public static class DatabaseFacadeExtensions
{
	public static async Task<bool> ExistsAsync(this DatabaseFacade source)
	{
		var exists = false;
		try
		{
			exists = await source.GetService<IRelationalDatabaseCreator>().ExistsAsync();
		}
		catch (Exception)
		{
		}

		return exists;
	}
}