using System;
using System.ComponentModel.DataAnnotations;

namespace TIMS_X.Server.Models;

public class ApiUrl
{
	public DateTime DateCreated { get; set; }
	public string Description { get; set; }
	public int Id { get; set; }
	public bool Inactive { get; set; }

	[Required] [StringLength(128)] public string Url { get; set; }
}