using System;
using Microsoft.AspNetCore.Http;

namespace TIMS_X.Server.Models;

public class DocumentUploadPayload
{
	//public List<IFormFile> Files { get; set; }
	public IFormFile Files { get; set; }
	public Guid PatientGuid { get; set; }
}