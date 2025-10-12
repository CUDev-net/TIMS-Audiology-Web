using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace TIMS_X.Server.Models;

public class SupportUser
{
	public SupportUser()
	{
		CustomersUpdated = new HashSet<Customer>();
	}

	public bool CanDelete => CustomersUpdated == null || !CustomersUpdated.Any();

	public ICollection<Customer> CustomersUpdated { get; set; }
	public DateTime DateCreated { get; set; }
	public bool DisableDelete => CustomersUpdated != null && CustomersUpdated.Any();

	[Required]
	[EmailAddress]
	[Remote("ValidateSupportUserEmail", "Validation", AdditionalFields = nameof(Id))]
	public string Email { get; set; }

	public int Id { get; set; }

	[Required] public bool Inactive { get; set; }

	[Required]
	[StringLength(64)]
	[Remote("ValidateSupportUserName", "Validation", AdditionalFields = nameof(Id))]
	public string Name { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }
}