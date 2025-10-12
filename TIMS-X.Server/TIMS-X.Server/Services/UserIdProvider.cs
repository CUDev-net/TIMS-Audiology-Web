using System.Linq;
using Microsoft.AspNetCore.SignalR;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Services;

public class UserIdProvider : IUserIdProvider
{
	public string GetUserId(HubConnectionContext connection)
	{
		var officeCode = connection.User?.Claims
			.Where(x => x.Type == StringConstants.OfficeCode)
			.Select(x => x.Value)
			.FirstOrDefault();
		if (string.IsNullOrWhiteSpace(officeCode))
			return null;
		var userId = connection.User?.Claims
			.Where(x => x.Type == StringConstants.User)
			.Select(x => x.Value)
			.FirstOrDefault();
		if (string.IsNullOrWhiteSpace(userId))
			return null;
		return $"{officeCode}-{userId}".ToLower();
	}
}