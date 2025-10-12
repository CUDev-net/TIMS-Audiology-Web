using Microsoft.AspNetCore.Http;

namespace TIMS_X.Server.Models;

public class DocumentUpload
{
	public string AlertMessage { get; set; }
	public IFormFile Document { get; set; }
	public string DocumentTitle { get; set; }
	public int PatientId { get; set; }
}