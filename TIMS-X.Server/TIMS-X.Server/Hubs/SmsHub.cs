using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TIMS_X.Core.Utils;

namespace TIMS_X.Server.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class SmsHub : Hub
{
	private static readonly HashSet<string> _users = new();

	public static bool IsUserConnected(string userId)
	{
		return _users.Contains(userId);
	}

	public override async Task OnConnectedAsync()
	{
		_users.Add(Context.UserIdentifier.ToLower());
		await base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception exception)
	{
		_users.Remove(Context.UserIdentifier.ToLower());
		return base.OnDisconnectedAsync(exception);
	}
}