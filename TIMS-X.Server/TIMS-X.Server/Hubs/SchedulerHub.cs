using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.Server.Hubs;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
public class SchedulerHub : Hub<ISchedulerHub>
{
	private static readonly ConcurrentDictionary<string, HashSet<string>> OfficeConnections = new();

	private async Task _Join(string officeCode)
	{
		var connectionId = Context.ConnectionId;

		// Check whether we have registered this office.
		HashSet<string> connectionIds;
		if (OfficeConnections.TryGetValue(officeCode, out connectionIds))
		{
			// If so, the add the connection if it doesn't yet exist.
			if (!connectionIds.Contains(connectionId))
				connectionIds.Add(connectionId);
		}
		else
		{
			// If not, then initialize the office and add the connection.
			OfficeConnections[officeCode] = new HashSet<string> { connectionId };
		}

		// Group this connection id by the office code
		await Groups.AddToGroupAsync(connectionId, officeCode);
	}

	public override async Task OnConnectedAsync()
	{
		var httpContext = Context.GetHttpContext();
		// Has to be lowercase
		var officeCode = httpContext.Request.Query["office"][0].ToLower();

		await _Join(officeCode);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		var connectionId = Context.ConnectionId;

		foreach (var office in OfficeConnections)
		{
			var officeCode = office.Key;
			var connectionIds = office.Value;

			if (connectionId.Contains(connectionId))
			{
				connectionIds.Remove(connectionId);
				await Groups.RemoveFromGroupAsync(connectionId, officeCode);

				break;
			}
		}

		await base.OnDisconnectedAsync(exception);
	}
}

public interface ISchedulerHub
{
	Task OnAppointmentCreated(AppointmentDto appointment);
	Task OnAppointmentDeleted(int id);
	Task OnAppointmentUpdated(AppointmentDto appointment);
	Task OnScheduleCreated(ScheduleDto schedule);
	Task OnScheduleDeleted(int id);
	Task OnScheduleUpdated(ScheduleDto schedule, RecurringIntervalRemoved recurringIntervalRemoved);
}