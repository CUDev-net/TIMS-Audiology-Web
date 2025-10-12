using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace TIMS_X.Server.Models;

public class Vendor
{
	public Vendor()
	{
		CustomerPermissions = new HashSet<CustomerVendorPermission>();
		DefaultPermissions = new HashSet<DefaultVendorPermission>();
	}

	[Display(Name = "API Key")]
	[Required]
	[StringLength(256)]
	[Remote("ValidateVendorApiKey", "Validation", AdditionalFields = nameof(Id))]
	public string ApiKey { get; set; }

	public bool CanDelete => CustomerPermissions == null || !CustomerPermissions.Any();

	public ICollection<CustomerVendorPermission> CustomerPermissions { get; set; }
	public DateTime DateCreated { get; set; }
	public ICollection<DefaultVendorPermission> DefaultPermissions { get; set; }

	public string DefaultPermissionsJson { get; set; }
	public bool DisableDelete => CustomerPermissions != null && CustomerPermissions.Any();
	public int Id { get; set; }

	[Required] public bool Inactive { get; set; }

	[Required]
	[StringLength(64)]
	[Remote("ValidateVendorName", "Validation", AdditionalFields = nameof(Id))]
	public string Name { get; set; }
}