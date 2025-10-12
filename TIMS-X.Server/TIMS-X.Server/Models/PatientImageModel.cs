using System;
using System.ComponentModel.DataAnnotations;

namespace TIMS_X.Server.Models;

public class PatientImageModel
{
	[Required] [StringLength(50)] public string Description { get; set; }

	[Required] public Guid DocumentTypeId { get; set; }

	public byte[] Image { get; set; }

	public string Name { get; set; }
}