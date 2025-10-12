using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class UserManagementHub : Hub
{
	private static readonly Dictionary<string, List<Tuple<int, DateTime>>> _users = new();

	public static List<Tuple<int, DateTime>> GetSignedInUsers(string officeCode)
	{
		if (_users.ContainsKey(officeCode)) return _users[officeCode];
		return new List<Tuple<int, DateTime>>();
	}

	public static bool IsUserConnected(string officeCode, int userId)
	{
		return _users.ContainsKey(officeCode) && _users[officeCode].Any(x => x.Item1 == userId);
	}

	public override async Task OnConnectedAsync()
	{
		var parts = Context.UserIdentifier.Split("-");
		var officeCode = parts[0];
		var userId = int.Parse(parts[1]);
		var signInDate = DateTime.Now;

		if (!_users.ContainsKey(officeCode)) _users[officeCode] = new List<Tuple<int, DateTime>>();
		var existingUser = _users[officeCode].FirstOrDefault(x => x.Item1 == userId);
		if (existingUser != null) _users[officeCode].Remove(existingUser);
		_users[officeCode].Add(new Tuple<int, DateTime>(userId, signInDate));

		await base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception exception)
	{
		var parts = Context.UserIdentifier.Split("-");
		var officeCode = parts[0];
		var userId = int.Parse(parts[1]);
		if (_users.ContainsKey(officeCode))
		{
			var existingUser = _users[officeCode].FirstOrDefault(x => x.Item1 == userId);
			if (existingUser != null) _users[officeCode].Remove(existingUser);
		}

		return base.OnDisconnectedAsync(exception);
	}
}