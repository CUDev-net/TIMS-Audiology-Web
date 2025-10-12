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
public class PatientPhotoHub : Hub
{
	public static string PhotoUpdated = "PhotoUpdated";
	public static string PhotoUpdated2 = "PhotoUpdated2";
	public static string PhotoUpdated3 = "PhotoUpdated3";
	public static string InsuranceCardUpdated = "InsuranceCardUpdated";
	public static string InsuranceCardUpdated2 = "InsuranceCardUpdated2";
	public static string PatientIntakeSubmitted = "PatientIntakeSubmitted";

	private static readonly HashSet<string> _users = new();

	public static bool IsUserConnected(string userId)
	{
		return _users.Contains(userId);
	}

	public override async Task OnConnectedAsync()
	{
		var officeCode = Context.UserIdentifier.Split("-").First().ToLower();
		await Groups.AddToGroupAsync(Context.ConnectionId, officeCode);
		_users.Add(Context.UserIdentifier.ToLower());
		await base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception exception)
	{
		var officeCode = Context.UserIdentifier.Split("-").First().ToLower();
		_users.Remove(Context.UserIdentifier.ToLower());
		Groups.RemoveFromGroupAsync(Context.ConnectionId, officeCode);
		return base.OnDisconnectedAsync(exception);
	}
}