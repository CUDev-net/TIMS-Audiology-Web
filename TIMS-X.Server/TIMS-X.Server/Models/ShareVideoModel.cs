using System;

namespace TIMS_X.Server.Models;

public class ShareVideoModel
{
	public bool AccessEnabled { get; set; }
	public DateTime? ExpirationDate { get; set; }
	public string Password { get; set; }
	public int VideoId { get; set; }
}