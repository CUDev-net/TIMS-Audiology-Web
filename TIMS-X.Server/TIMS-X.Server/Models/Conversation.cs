using System;

namespace TIMS_X.Server.Models;

public class Conversation
{
	public DateTime ComparableDate => DateLastReceived.HasValue ? DateLastReceived.Value : DateCreated;
	public DateTime DateCreated { get; set; }
	public DateTime? DateLastReceived { get; set; }
	public int MessageCount { get; set; }
	public int PatientId { get; set; }
	public string PhoneNumber { get; set; }
	public int UserId { get; set; }
}