using System;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.Dtos;

public class PatientAppointmentItemDto
{
	public PatientAppointmentItemDto(Appointment appointment)
	{
		AppointmentId = appointment.Id;
		Date = appointment.StartsAt;
		DateReceived = appointment.CreatedDate;
		var delimitter = Environment.NewLine;
		if (!appointment.Notes.Contains(Environment.NewLine))
			delimitter = "\n";
		foreach (var line in appointment.Notes.Split(new[] { delimitter }, StringSplitOptions.None))
			if (line.StartsWith("New Patient") || line.StartsWith("Existing Patient"))
			{
				var names = line.Split(' ');
				if (names.Length == 4)
				{
					FirstName = names[2];
					LastName = names[3];
				}

				if (names.Length == 5)
				{
					FirstName = names[2];
					Initial = names[3];
					LastName = names[4];
				}
			}
			else if (line.StartsWith("Phone:"))
			{
				Phone = _GetValueForToken(line);
			}
			else if (line.StartsWith("Email:"))
			{
				Email = _GetValueForToken(line);
			}
			else if (line.StartsWith("DOB:"))
			{
				var value = _GetValueForToken(line);
				if (DateTime.TryParse(value, out var d))
						BirthDate = d;
			}
	}

	public int AppointmentId { get; }
	public DateTime BirthDate { get; }
	public DateTime Date { get; }
	public DateTime DateReceived { get; }
	public string Email { get; }
	public string FirstName { get; }
	public string Initial { get; }
	public string LastName { get; }
	public string Phone { get; }
	public string Provider { get; set; }
	public string Site { get; set; }
	
	private string _GetValueForToken(string tokenAndValue)
	{
		var delimiter = ' ';
		var index = tokenAndValue.IndexOf(delimiter);
		var result = tokenAndValue.Substring(index + 1);
		return result;
	}
}