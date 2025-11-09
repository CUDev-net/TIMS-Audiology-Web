using System;
using System.ComponentModel.DataAnnotations;

namespace TIMS_X.Server.Models;

public class TimsLog
{
	public TimsLog()
	{
		DateCreated = DateTime.Now;
	}

	public string OfficeCode { get; set; }
	public DateTime DateCreated { get; set; }

	[StringLength(256)] public string Error { get; set; }

	public int Id { get; set; }

	[StringLength(256)] public string Message { get; set; }
}