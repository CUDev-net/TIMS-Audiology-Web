using System.ComponentModel.DataAnnotations;

namespace TIMS_X.Server.Models;

public class ConnectionDetails
{
	public string Database { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	public string Server { get; set; }

	[Required] public string User { get; set; }
}