using Newtonsoft.Json;

namespace TIMS_X.Server.Models;

public class ErrorResponse
{
	public string ErrorMessage { get; set; }

	// other fields

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}