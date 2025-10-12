using System;
using TIMS_X.Core.Enums;

namespace TIMS_X.Server.Models;

public class FormLink
{
	public Customer Customer { get; set; }
	public int CustomerId { get; set; }
	public DateTime DateCreated { get; set; }
	public PatientFormTypeEnum FormType { get; set; }
	public int Id { get; set; }
	public int PatientId { get; set; }
	public bool Submitted { get; set; }
	public string Url { get; set; }
	public int UserId { get; set; }
}