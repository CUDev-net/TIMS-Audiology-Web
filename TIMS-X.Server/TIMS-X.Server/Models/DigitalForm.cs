using System;
using System.Text.Json.Serialization;
using TIMS_X.Core.Enums;

namespace TIMS_X.Server.Models;

public class DigitalForm
{
	public string EditableForm { get; set; }
	public PatientFormTypeEnum FormType { get; set; }
	public string LiveForm { get; set; }

	[JsonIgnore] public DateTime UpdatedDate { get; set; }

	[JsonIgnore] public string UpdatedUser { get; set; }
}